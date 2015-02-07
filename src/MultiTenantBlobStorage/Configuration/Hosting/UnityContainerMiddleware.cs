using Microsoft.Owin;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting
{
    public class UnityContainerMiddleware
    {
        readonly private Func<IDictionary<string, object>, Task> _next;
        readonly private IUnityContainer _container;

        public UnityContainerMiddleware(Func<IDictionary<string, object>, Task> next, IUnityContainer container)
        {
            _next = next;
            _container = container;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            // this creates a per-request, disposable scope
            using (var scope = _container.CreateChildContainer())
            {
                scope.RegisterInstance<IOwinContext>(new OwinContext(env));
                // this makes scope available for downstream frameworks
                env[Constants.OwinContextProperties.UnityRequestContainer] = scope;
                await _next(env);
            }
        }
    }
}
