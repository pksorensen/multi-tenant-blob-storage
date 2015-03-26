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
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Globalization;

namespace SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting
{
    public class ResourceContext
    {

        public ResourceContext()
        {
           

        }

        public TenantRoute Route { get; set; }
        public string Action { get; set; }

        public ResourceAuthorizationContext ResourceAuthorizationContext
        {
            get
            {
                return new ResourceAuthorizationContext(this.User, new []{ new Claim("action", Action)}, new []{new Claim("tenant", Route.TenantId),new Claim("resource", Route.ContainerName??"")});
            }
        }

        public ClaimsPrincipal User { get; set; }
    }
    internal class StorageMirrowMiddleware
    {
        static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        readonly Func<IDictionary<string, object>, Task> _next;


      //  protected OwinContext Context { get; set; }
       // protected ResourceContext ResourceContext { get; set; }
       // protected MultiTenantBlobStorageOptions Options { get; set; }
        public StorageMirrowMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
            
        }
        public async Task Invoke(IDictionary<string, object> env)
        {
            var context = new OwinContext(env);
            var resourceContext = new ResourceContext();
            context.Response.OnSendingHeaders((obj) =>
            {
               // Trace.WriteLine("SENDING HEADERS");
            }, null);

            var options = context.ResolveDependency<MultiTenantBlobStorageOptions>();
            var requestHandler = context.ResolveDependency<IRequestHandlerService>();

         //   requestHandler.ListBlobsStreamingTransform = TestStreamTransform;

            resourceContext.User = context.Authentication.User = await requestHandler.AuthenticateRequestAsync(context.Request, options) ?? new ClaimsPrincipal();
            resourceContext.Route = await requestHandler.ParseRouteDataAsync(context.Request, options);
            resourceContext.Action = string.Format("{0}_{1}{2}",
                resourceContext.Route.Path.IsMissing() ? (resourceContext.Route.Resource.IsMissing() ? Constants.Actions.TenantPrefix : Constants.Actions.ContainerPrefix) : Constants.Actions.BlobPrefix,
                context.Request.Method.ToLower(),
                context.Request.Query["comp"].IsPresent() ? "_" + context.Request.Query["comp"] : "");

            if(!resourceContext.User.Identities.Any())
            {
        
                var sig = context.Request.Query["s"];
                var expire = context.Request.Query["e"];
                var ac = context.Request.Query["ac"];
                var a = context.Request.Query["a"];
                var expireTIme = DateTimeOffset.Parse(expire);
                String dateInRfc1123Format = expireTIme.ToString("R", CultureInfo.InvariantCulture);
                
                if(!(string.IsNullOrWhiteSpace(sig) || string.IsNullOrWhiteSpace(expire)))
                {
                    var account = context.ResolveDependency<IStorageAccountResolverService>().GetStorageAccount(resourceContext.Route);

                    string signature = "";
                    using (HMACSHA256 hmacSha256 = new HMACSHA256(Convert.FromBase64String(account.Credentials.ExportBase64EncodedKey())))
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine(ac);
                        sb.AppendLine(resourceContext.Route.TenantId);
                        sb.AppendLine(resourceContext.Route.Resource);
                        if (a == "b")
                        {
                            sb.AppendLine(resourceContext.Route.Path);
                        }
                        sb.AppendLine(dateInRfc1123Format);


                        Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                        signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
                        if (sig == signature)
                        {
                            resourceContext.User = context.Authentication.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{ new Claim("signature",signature)},"signature"));

                        }

                    }
                }

            }



            if (await context.CheckAccessAsync(resourceContext.ResourceAuthorizationContext))
            {

                await requestHandler.HandleAsync(context, resourceContext);

            }
            else
            {

                HandleUnauthorizedRequest( context, resourceContext.ResourceAuthorizationContext);
            }

            context.Response.Body.Flush();

            //await _next(env);
        }
       


      
       
        protected void HandleUnauthorizedRequest( IOwinContext context, ResourceAuthorizationContext actionContext)
        {
            if (actionContext.Principal != null &&
                actionContext.Principal.Identity != null &&
                actionContext.Principal.Identity.IsAuthenticated)
            {
                // actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Forbidden");

                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ReasonPhrase = "Forbidden";

            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ReasonPhrase = "Unauthorized";
            }
        }



       
    }
}
