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

            var fact = options.Factory ?? new MultiTenantBlobStorageServiceFactory();

            container.RegisterDefaultType<ITenantContainerNameService, DefaultTenantContainerNameService>(fact.TenantContainerNameService);
            container.RegisterDefaultType<IStorageAccountResolverService, DefaultStorageAccountResolverService>(fact.StorageAccountResolver);
            
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
        private static void RegisterDefaultType<T, TDefault>(this IUnityContainer container, Registration<T> registration, string name = null)
            where T : class
            where TDefault : T
        {
            if (registration != null)
            {
                container.Register(registration, name);
            }
            else
            {
                if (name == null)
                {
                    container.RegisterType<T, TDefault>(new HierarchicalLifetimeManager());
                }
                else
                {
                    container.RegisterType<T, TDefault>(name, new HierarchicalLifetimeManager());
                }
            }
        }

        private static void Register(this IUnityContainer container, Registration registration, string name = null)
        {
            if (registration.Instance != null)
            {
                var reg = container.RegisterInstance(registration.Instance);
                if (name != null)
                {
                    container.RegisterInstance(name,registration.Instance);
                 
                }
                else
                {
                    container.RegisterInstance(registration.DependencyType, registration.Instance);
                }
            }
            else if (registration.Type != null)
            {
              
                if (name != null)
                {
                    container.RegisterType(registration.DependencyType, registration.Type, name, new HierarchicalLifetimeManager());
                }
                else
                {
                    container.RegisterType(registration.Type,registration.DependencyType,new HierarchicalLifetimeManager());
                }
            }
            else if (registration.Factory != null)
            {
               
                if (name != null)
                {
                    container.RegisterType(registration.DependencyType, registration.Type, name, new HierarchicalLifetimeManager(),new InjectionFactory((c) => registration.Factory(new UnityDependencyResolver(c))));
                }
                else
                {
                    container.RegisterType(registration.DependencyType, registration.Type, new HierarchicalLifetimeManager(), new InjectionFactory((c) => registration.Factory(new UnityDependencyResolver(c))));
                }
            }
            else
            {
                var message = "No type or factory found on registration " + registration.GetType().FullName;
                Logger.Error(message);
                throw new InvalidOperationException(message);
            }
        }
    }
}
