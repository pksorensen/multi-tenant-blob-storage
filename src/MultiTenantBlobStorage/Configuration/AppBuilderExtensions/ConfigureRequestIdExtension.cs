using Owin;
using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SInnovations.Azure.MultiTenantBlobStorage.Extensions;


namespace Owin
{
    internal static class ConfigureRequestIdExtension
    {


        internal static IAppBuilder ConfigureRequestId(this IAppBuilder app)
        {
            app.Use(async (ctx, next) =>
            {

                if (Trace.CorrelationManager.ActivityId == Guid.Empty)
                {
                    Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                }

                var id = Trace.CorrelationManager.ActivityId.ToString();
                ctx.Environment.SetRequestId(id);

                await next();
            });

            return app;
        }
    }
}

