using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SInnovations.Azure.MultiTenantBlobStorage.Extensions;
using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using SInnovations.Azure.MultiTenantBlobStorage.Services;
using Microsoft.Owin;
using System.Net;
using System.IO;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;
using System.Security.Claims;
using SInnovations.Azure.MultiTenantBlobStorage.Notifications;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using System.Xml;
using StorageConstants = Microsoft.WindowsAzure.Storage.Shared.Protocol.Constants;
using System.Xml.Linq;

namespace SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting
{
    public class ResourceContext
    {

        public ResourceContext()
          
        {
            User = new ClaimsPrincipal();
          
        }
        public HttpWebRequest Request { get; set; }
        public TenantRoute Route { get; set; }
        public string Action { get; set; }

        public ResourceAuthorizationContext ResourceAuthorizationContext
        {
            get
            {
                return new ResourceAuthorizationContext(this.User, Action, Route.ContainerName);
            }
        }

        public ClaimsPrincipal User { get; set; }
    }
    internal class StorageMirrowMiddleware
    {
        static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        readonly Func<IDictionary<string, object>, Task> _next;

      
        protected OwinContext Context { get; set; }
        protected ResourceContext ResourceContext { get; set; }
        protected MultiTenantBlobStorageOptions Options { get; set; }
        public StorageMirrowMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
            ResourceContext = new ResourceContext();
        }
        public async Task Invoke(IDictionary<string, object> env)
        {
            Context = new OwinContext(env);

            Options = Context.ResolveDependency<MultiTenantBlobStorageOptions>();
            var result = await Context.Authentication.AuthenticateAsync(Options.AuthenticationType);
            if(result!=null)
            {

                ResourceContext.User = new ClaimsPrincipal(result.Identity);
                
            }

            ResourceContext.Route = await Context.ResolveDependency<IRequestTenantResolver>().GetRouteAsync(Context.Request);

            if(Context.Request.Method == Constants.HttpMethods.Put)
            {
                if (ResourceContext.Route.Path.IsMissing())
                {
                    ResourceContext.Action = Constants.Actions.ContainerCreate;
                }
            }else if(Context.Request.Method == Constants.HttpMethods.Head)
            {
                if (ResourceContext.Route.Path.IsMissing())
                {
                    ResourceContext.Action = Constants.Actions.ContainerExists;
                }
            }
            else if (Context.Request.Method == Constants.HttpMethods.Delete)
            {
                if (ResourceContext.Route.Path.IsMissing())
                {
                    ResourceContext.Action = Constants.Actions.ContainerDelete;
                }
            }
            else if (Context.Request.Method == Constants.HttpMethods.Get)
            {
                if (ResourceContext.Route.Path.IsMissing())
                {
                    ResourceContext.Action = Constants.Actions.ContainerList;
                }
            }
            else
            {
                ResourceContext.Action = "NotHandled";
            }
           
            
            
            ResourceContext.Request = BuildBlobStorageRequest();

            Logger.LogRequest(ResourceContext.Request);

            if(await Context.CheckAccessAsync(ResourceContext.ResourceAuthorizationContext))
            {
               
            

            var blobAuthenticationService = Context.ResolveDependency<IBlobAuthenticationService>();
            await blobAuthenticationService.SignRequestAsync(ResourceContext.Request, ResourceContext.Route);



            await ExecuteAsync();

            }
            else
            {

                HandleUnauthorizedRequest(ResourceContext.ResourceAuthorizationContext);
            }


            //await _next(env);
        }

        protected void HandleUnauthorizedRequest(ResourceAuthorizationContext actionContext)
        {
            if (actionContext.Principal != null &&
                actionContext.Principal.Identity != null &&
                actionContext.Principal.Identity.IsAuthenticated)
            {
                 // actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Forbidden");

                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.ReasonPhrase = "Forbidden";
    
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                Context.Response.ReasonPhrase = "Unauthorized";
            }
        }


       
        public async Task ExecuteAsync()
        {
           

            
                if(ResourceContext.Request.ContentLength > 0)
                {
                    using (var stream = await ResourceContext.Request.GetRequestStreamAsync())
                    {
                        byte[] buffer = new byte[81920];
                        int count;
                        while ((count = await Context.Request.Body.ReadAsync(buffer, 0, buffer.Length, Context.Request.CallCancelled)) != 0)
                        {
                            Context.Request.CallCancelled.ThrowIfCancellationRequested();

                            await stream.WriteAsync(buffer, 0, count, Context.Request.CallCancelled);
                        }
                    }
                }


                using (HttpWebResponse response = await GetResponseAsync())
                {

                    Context.Response.ReasonPhrase = response.StatusDescription;
                    Context.Response.StatusCode = (int)response.StatusCode;

                    BaseNotification baseNotification = new BaseNotification
                    {
                        Resource = ResourceContext.Route.Resource,
                        Tenant = ResourceContext.Route.TenantId
                    };

                    
                    foreach (var headerKey in response.Headers.AllKeys)
                    {
                        Context.Response.Headers.Add(headerKey, response.Headers.GetValues(headerKey));
                    }



                    if (response.ContentLength != 0)
                    {
                        var stream = response.GetResponseStream();
                        if(Options.ListBlobOptions.BlobListFilter != null && ResourceContext.Action == Constants.Actions.ContainerList)
                        {
                            try
                            {
                              
                                XmlReaderSettings readerSettings = new XmlReaderSettings();
                                readerSettings.IgnoreWhitespace = false;
                                var reader = XmlReader.Create(stream, readerSettings);
                                using (var writer = XmlWriter.Create(Context.Response.Body, new XmlWriterSettings { Async = true , CloseOutput=false}))
                                {
                                    while (reader.Read())
                                    {
                                        while (reader.Name == "Blob")
                                        {
                                            var el = (XElement)XNode.ReadFrom(reader);
                                            if (Options.ListBlobOptions.BlobListFilter(el))
                                            {
                                                el.WriteTo(writer);
                                                await writer.FlushAsync();
                                            }


                                        }

                                        WriteShallowNode(reader, writer);
                                    }
                                }
                               
                                
                            }catch(Exception ex)
                            {

                            }


                            
                             
                        }else                        
                        {

                           
                            
                            byte[] buffer = new byte[81920];
                            int count;
                            while ((count = await stream.ReadAsync(buffer, 0, buffer.Length, Context.Request.CallCancelled)) != 0)
                            {
                                Context.Request.CallCancelled.ThrowIfCancellationRequested();
                               
                                await Context.Response.Body.WriteAsync(buffer, 0, count, Context.Request.CallCancelled);
                            }
                          
                            
                        }
                        
                        // new ListContainersResponse()  


                    }




                }
            

        }

        protected async Task<HttpWebResponse> GetResponseAsync()
        {
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)await ResourceContext.Request.GetResponseAsync();

            }
            catch (WebException e)
            {
                response = (HttpWebResponse)e.Response;
            }
            return response;
        }

        protected HttpWebRequest BuildBlobStorageRequest()
        {
            
           
            
            var request = CreateHttpWebRequest();
            var req = Context.Request;            

            if ((req.Method == Constants.HttpMethods.Post || req.Method == Constants.HttpMethods.Put))
            {
               // request.ContentLength = req.Body.Length;
                req.Headers.CopyTo(Constants.HeaderConstants.ContentLenght, request);
            }
               
            req.Headers.CopyTo(Constants.HeaderConstants.UserAgent, request);
            req.Headers.CopyTo(Constants.HeaderConstants.ContentType, request);
            req.Headers.CopyTo(Constants.HeaderConstants.Date, request);         

            foreach (var header in req.Headers.Keys.Where(k => k.StartsWith("x-ms")))
            {
                req.Headers.CopyTo(header, request);
               
            }
                        
            request.Host = "ascendxyzweutest.blob.core.windows.net";

            
            
           

            return request;

        }

        protected HttpWebRequest CreateHttpWebRequest()
        {
           
            var resourceId = string.Format("{0}/{1}", ResourceContext.Route.TenantId, ResourceContext.Route.Resource);
            var idx = Context.Request.Uri.AbsoluteUri.IndexOf(resourceId) + resourceId.Length;
            var pathAndQuery = Context.Request.Uri.AbsoluteUri.Substring(idx);
            var qIdx = pathAndQuery.IndexOf('?');
            if(qIdx>=0)
            {
               // ResourceContext.Action
            }

                ResourceContext.Route.Path = qIdx>-1 ? pathAndQuery.Substring(0, qIdx) : pathAndQuery;
            
            var uri = new Uri(ResourceContext.Route.Host + ResourceContext.Route.ContainerName + pathAndQuery);
            // var request = new HttpRequestMessage(msg.Method, uri);
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = Context.Request.Method;
            return request;
        }

        void WriteShallowNode(XmlReader reader, XmlWriter writer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                    writer.WriteAttributes(reader, true);
                    if (reader.IsEmptyElement)
                    {
                        writer.WriteEndElement();
                    }
                    break;
                case XmlNodeType.Text:
                    writer.WriteString(reader.Value);
                    break;
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    writer.WriteWhitespace(reader.Value);
                    break;
                case XmlNodeType.CDATA:
                    writer.WriteCData(reader.Value);
                    break;
                case XmlNodeType.EntityReference:
                    writer.WriteEntityRef(reader.Name);
                    break;
                case XmlNodeType.XmlDeclaration:
                case XmlNodeType.ProcessingInstruction:
                    writer.WriteProcessingInstruction(reader.Name, reader.Value);
                    break;
                case XmlNodeType.DocumentType:
                    writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                    break;
                case XmlNodeType.Comment:
                    writer.WriteComment(reader.Value);
                    break;
                case XmlNodeType.EndElement:
                    writer.WriteFullEndElement();
                    break;
            }
        }
    }
}
