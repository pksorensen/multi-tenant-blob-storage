using SInnovations.Azure.MultiTenantBlobStorage.Configuration;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


            app.UseWebApi(WebApiConfig.Configure(options));


            return app;
        }
    }
}
