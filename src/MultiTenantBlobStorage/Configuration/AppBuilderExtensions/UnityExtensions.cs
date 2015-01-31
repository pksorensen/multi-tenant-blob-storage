using Microsoft.Practices.Unity;
using SInnovations.Azure.MultiTenantBlobStorage;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin
{
    public static class UnityExtensions
    {
        public static IUnityContainer UseUnityContainer(this IAppBuilder app)
        {

            if (app.Properties.ContainsKey(Constants.OwinAppBuilderProperties.UnityContainer))
                return app.GetUnityContainer();

            var container = new UnityContainer();
            app.Properties.Add(Constants.OwinAppBuilderProperties.UnityContainer, container);
            app.Use(typeof(UnityContainerMiddleware), container);
            return container;
        }
        public static IUnityContainer GetUnityContainer(this IAppBuilder app)
        {
            return app.Properties[Constants.OwinAppBuilderProperties.UnityContainer] as IUnityContainer;
        }
    }
}
