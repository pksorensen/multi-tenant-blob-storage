using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.Default
{
    internal class UnityDependencyResolver : IDependencyResolver
    {
        readonly IUnityContainer ctx;
        public UnityDependencyResolver(IUnityContainer ctx)
        {
            this.ctx = ctx;
        }

        public T Resolve<T>(string name)
        {
            if (name != null)
            {
                return ctx.Resolve<T>(name);
            }

            return ctx.Resolve<T>();
        }
    }
}
