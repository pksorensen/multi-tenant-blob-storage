using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration;
using SInnovations.Azure.MultiTenantBlobStorage;
using System.Text;
using SInnovations.Azure.MultiTenantBlobStorage.Logging;

[assembly: OwinStartup(typeof(Host.Startup))]

namespace Host
{
    public class Startup
    {
        static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            app.Map(Constants.OwinDefaultMapping, (builder) =>
            {
                builder.Use(async (ctx, next) =>
                {
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine();
                        sb.AppendLine(string.Format("[{0}] {1}", ctx.Request.Method, ctx.Request.Uri));
                        foreach (var header in ctx.Request.Headers)
                        {
                            sb.AppendLine(string.Format("{0}: {1}", header.Key, string.Join(", ", header.Value)));
                        }
                        Logger.Info(sb.ToString());
                    }

                    
                    await next();

                    {
                        var sb = new StringBuilder();
                        sb.AppendLine();
                        sb.AppendLine(string.Format("[{0}] {1}", ctx.Response.StatusCode, ctx.Response.ReasonPhrase));
                        foreach (var header in ctx.Response.Headers)
                        {
                            sb.AppendLine(string.Format("{0}: {1}", header.Key, string.Join(", ", header.Value)));
                        }
                        Logger.Info(sb.ToString());
                    }


                });
                builder.UseMultiTenantBlobStorage(new MultiTenantBlobStorageOptions
                {
                    ContainerResourceName = "workset",
                });
            });
        }
    }
}
