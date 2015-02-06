using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.Default
{
    class DefaultStorageAccountResolverService : IStorageAccountResolverService
    {
        public Task<string> GetBlobEndpointAsync(TenantRoute route)
        {
            return Task.FromResult("https://ascendxyzweutest.blob.core.windows.net/");
        }
    }
}
