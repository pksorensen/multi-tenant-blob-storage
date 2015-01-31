using Microsoft.Practices.Unity;
using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Hosting;

namespace SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting
{
    public class KatanaDependencyResolver : DelegatingHandler
    {
        static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var owin = request.GetOwinContext();
            var scope = owin.Get<IUnityContainer>(Constants.OwinContextProperties.UnityRequestContainer);
            if (scope != null)
            {
                request.Properties[HttpPropertyKeys.DependencyScope] = new UnityScope(scope);
            }
            else
            {
                Logger.WarnFormat("{0} not regiested (no dependency injeciton on controllers), have you run UseUnityContainer");

            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
