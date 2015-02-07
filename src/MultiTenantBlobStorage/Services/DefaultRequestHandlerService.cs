using Microsoft.Owin;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SInnovations.Azure.MultiTenantBlobStorage.Extensions;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using System.Net;
using SInnovations.Azure.MultiTenantBlobStorage.Notifications;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Threading;
using SInnovations.Azure.MultiTenantBlobStorage.Services.RequestHandlers;
namespace SInnovations.Azure.MultiTenantBlobStorage.Services
{
    
    
   
   
   
   
    public class DefaultRequestHandlerService : IRequestHandlerService
    {
        static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        protected MultiTenantBlobStorageOptions Options { get; set; }

        public DefaultRequestHandlerService(MultiTenantBlobStorageOptions options)
        {
            Options = options;
        }

        //protected IListBlobsService 

       // public Func<Stream, Stream,ListBlobsRequestOptions, Task> ListBlobsStreamingTransform { get; set; }



        public virtual async Task<ClaimsPrincipal> AuthenticateRequestAsync(IOwinRequest request, MultiTenantBlobStorageOptions options)
        {
            var result = await request.Context.Authentication.AuthenticateAsync(options.AuthenticationType);
            if (result == null)
                return null;

            return new ClaimsPrincipal(result.Identity);
        }


        public virtual async Task<TenantRoute> ParseRouteDataAsync(IOwinRequest request, MultiTenantBlobStorageOptions options)
        {
            var route = await request.Context.ResolveDependency<IRequestTenantResolver>().GetRouteAsync(request);

            return route;
        }


        public Task HandleAsync(IOwinContext context, ResourceContext resourceContext)
        {
            var requestOption = new RequestOptions(context.Request);
            
            
            switch (resourceContext.Action)
            {
                case Constants.Actions.ListBlobs:
                    IListBlobsHandler listBlobs = context.ResolveDependency<IListBlobsHandler>();

                    if(listBlobs.ImplementsHandleRequest)
                    {
                        Logger.Info("Using BlobList Provider");
                        return listBlobs.HandleRequest(context, resourceContext, requestOption);

                    }else if (listBlobs.ImplementsRequestTransformer)
                    {
                        return ExecuteAsync(context, resourceContext, listBlobs.Transformer, requestOption);
                    }
                    break;
            }
            return ExecuteAsync(context, resourceContext);
            

           
        }

      

        /// <summary>
        /// Builds a httpwebrequset to carry out a mirrow to blob storage.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual HttpWebRequest BuildDefaultBlobStorageRequest(IOwinContext context, ResourceContext resourceContext)
        {
            
            var request = CreateHttpWebRequest(context.Request,resourceContext);
            var req = context.Request;

            if ((req.Method == Constants.HttpMethods.Post || req.Method == Constants.HttpMethods.Put))
            {
                // request.ContentLength = req.Body.Length;
                req.Headers.CopyTo(Constants.HeaderConstants.ContentLenght, request);
            }

            req.Headers.CopyTo(Constants.HeaderConstants.UserAgent, request);
            req.Headers.CopyTo(Constants.HeaderConstants.ContentType, request, "application/xml");
            req.Headers.CopyTo(Constants.HeaderConstants.Date, request);

            foreach (var header in req.Headers.Keys.Where(k => k.StartsWith("x-ms")))
            {
                req.Headers.CopyTo(header, request);

            }

            return request;

        }

        /// <summary>
        /// Create the HttpWebRequest to mirrow from blob storage, parsing and setting up url.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resourceContext"></param>
        /// <returns></returns>
        protected virtual HttpWebRequest CreateHttpWebRequest(IOwinRequest request, ResourceContext resourceContext)
        {

            var resourceId = string.Format("{0}/{1}", resourceContext.Route.TenantId, resourceContext.Route.Resource);
            var idx = request.Uri.AbsoluteUri.IndexOf(resourceId) + resourceId.Length;
            var pathAndQuery = request.Uri.AbsoluteUri.Substring(idx);
            var qIdx = pathAndQuery.IndexOf('?');
            if (qIdx >= 0)
            {
                // ResourceContext.Action
            }

            resourceContext.Route.Path = qIdx > -1 ? pathAndQuery.Substring(0, qIdx) : pathAndQuery;

            var uri = new Uri(resourceContext.Route.Host + resourceContext.Route.ContainerName + pathAndQuery);
            // var request = new HttpRequestMessage(msg.Method, uri);
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.Method = request.Method;
         
            return req;
        }
        private Task ExecuteAsync(IOwinContext context, ResourceContext resourceContext)
        {
            return ExecuteAsync<RequestOptions>(context, resourceContext);
        }
        private async Task ExecuteAsync<T>(IOwinContext context, ResourceContext resourceContext, Func<Stream, Stream, T, Task> provider = null, T options = null) where T : RequestOptions
        {
            var request = BuildDefaultBlobStorageRequest(context, resourceContext);
            Logger.LogRequest(request);
            var blobAuthenticationService = context.ResolveDependency<IStorageAccountResolverService>();
            await blobAuthenticationService.SignRequestAsync(request, resourceContext.Route);

            await ForwardIncomingRequestStreamAsync(context, request);

            using (HttpWebResponse response = await GetResponseAsync(request))
            {

                context.Response.ReasonPhrase = response.StatusDescription;
                context.Response.StatusCode = (int)response.StatusCode;

                foreach (var headerKey in response.Headers.AllKeys)
                {
                    context.Response.Headers.Add(headerKey, response.Headers.GetValues(headerKey));
                }



                if (response.ContentLength != 0)
                {
                    var stream = response.GetResponseStream();

                    if (provider == null)
                    {
                        await ForwardDefaultResponseStreamAsync(context, stream);
                    }
                    else
                    {
                        await provider(stream, context.Response.Body, options);
                       
                    }

                }
                
            }


        }

        private static async Task ForwardDefaultResponseStreamAsync(IOwinContext context, Stream stream)
        {
            byte[] buffer = new byte[81920];
            int count;
            while ((count = await stream.ReadAsync(buffer, 0, buffer.Length, context.Request.CallCancelled)) != 0)
            {
                context.Request.CallCancelled.ThrowIfCancellationRequested();

                await context.Response.Body.WriteAsync(buffer, 0, count, context.Request.CallCancelled);
            }
        }

        protected virtual async Task ForwardIncomingRequestStreamAsync(IOwinContext context, HttpWebRequest request)
        {
            if (request.ContentLength > 0)
            {
                using (var stream = await request.GetRequestStreamAsync())
                {
                    byte[] buffer = new byte[81920];
                    int count;
                    while ((count = await context.Request.Body.ReadAsync(buffer, 0, buffer.Length, context.Request.CallCancelled)) != 0)
                    {
                        context.Request.CallCancelled.ThrowIfCancellationRequested();

                        await stream.WriteAsync(buffer, 0, count, context.Request.CallCancelled);
                    }
                }
            }
        }

        private async Task<HttpWebResponse> GetResponseAsync(HttpWebRequest request)
        {
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();

            }
            catch (WebException e)
            {
                response = (HttpWebResponse)e.Response;
            }
            return response;
        }




    }
}
