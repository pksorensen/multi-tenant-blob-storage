using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services
{
    public interface IDependencyResolver
    {
        /// <summary>
        /// Resolves the dependency based upon the type. If name is provided then the dependency is also resolved by name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>The dependency.</returns>
        T Resolve<T>(string name = null);
    }
}
