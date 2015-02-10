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
            var requestHandler = Context.ResolveDependency<IRequestHandlerService>();

         //   requestHandler.ListBlobsStreamingTransform = TestStreamTransform;

            ResourceContext.User = await requestHandler.AuthenticateRequestAsync(Context.Request, Options) ?? new ClaimsPrincipal();
            ResourceContext.Route = await requestHandler.ParseRouteDataAsync(Context.Request, Options);
            ResourceContext.Action = string.Format("{0}_{1}", ResourceContext.Route.Path.Trim('/').IsMissing() ? (ResourceContext.Route.Resource.IsMissing()?Constants.Actions.TenantPrefix: Constants.Actions.ContainerPrefix) : Constants.Actions.BlobPrefix, Context.Request.Method.ToLower());
     
            if (await Context.CheckAccessAsync(ResourceContext.ResourceAuthorizationContext))
            {

                await requestHandler.HandleAsync(Context, ResourceContext);

            }
            else
            {

                HandleUnauthorizedRequest(ResourceContext.ResourceAuthorizationContext);
            }

            Context.Response.Body.Flush();

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



       
    }
}
