using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.Default
{
    public class DefaultTenantContainerNameService : ITenantContainerNameService
    {
        public Task<string> GetContainerNameAsync(TenantRoute route)
        {
            return Task.FromResult(string.Format("{0}-{1}", new Guid(route.TenantId).ToString("N"), route.Resource));
        }
    }
}
