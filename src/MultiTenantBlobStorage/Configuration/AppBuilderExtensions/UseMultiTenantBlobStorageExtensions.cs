using SInnovations.Azure.MultiTenantBlobStorage.Configuration;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using SInnovations.Azure.MultiTenantBlobStorage.Services;
using SInnovations.Azure.MultiTenantBlobStorage.Services.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Owin;
using System.Net.Http;
using SInnovations.Azure.MultiTenantBlobStorage;
using System.Net;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

using SInnovations.Azure.MultiTenantBlobStorage.Extensions;
using SInnovations.Azure.MultiTenantBlobStorage.Services.RequestHandlers;
using SInnovations.Azure.MultiTenantBlobStorage.Services.RequestHandlers.Default;

namespace Owin
{
    

    public static class UseMultiTenantBlobStorageExtensions
    {
        private static readonly ILog Logger = LogProvider.GetLogger("Startup");

        public static IAppBuilder UseMultiTenantBlobStorage(this IAppBuilder app,MultiTenantBlobStorageOptions options){

            if (app == null) throw new ArgumentNullException("app");
            if (options == null) throw new ArgumentNullException("options");

            if (options.RequireSsl)
            {
                app.Use<RequireSslMiddleware>();
            }

            app.ConfigureRequestId();


            var container = app.UseUnityContainer();

            container.RegisterType<ITenantContainerNameService, DefaultTenantContainerNameService>();
            container.RegisterType<IStorageAccountResolverService, DefaultStorageAccountResolverService>();
            container.RegisterType<IBlobAuthenticationService,DefaultBlobAuthenticationService>();
            container.RegisterType<IRequestTenantResolver, DefaultRequestTenantResolver>();
            container.RegisterType<IResourceAuthorizationManager, DefaultResourceAuthorizationManager>();
            container.RegisterType<IRequestHandlerService, DefaultRequestHandlerService>();
            container.RegisterType<IListBlobsHandler, DefaultListBlobsHandler>();

            app.UseResourceAuthorization(new ResourceAuthorizationMiddlewareOptions
            {
                ManagerProvider = (ctx) => ctx.ResolveDependency<IResourceAuthorizationManager>()
            });

            container.RegisterInstance(options);


            app.Use(async (ctx, next) =>
            {
           
                
                await next();

                if (options.ContainerResourceName.IsPresent())
                {
                    
                    var idx = ctx.Response.ReasonPhrase.IndexOf("container");
                    if (idx > -1)
                    {
                        ctx.Response.ReasonPhrase = ctx.Response.ReasonPhrase.Substring(0, idx) +
                            options.ContainerResourceName + ctx.Response.ReasonPhrase.Substring(idx + 9);
                    }
                   
                }
            });
            app.Use<StorageMirrowMiddleware>();
          //  app.UseWebApi(WebApiConfig.Configure(options));


           

            return app;
        }
    }
}
