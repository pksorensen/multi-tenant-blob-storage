using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SInnovations.Azure.MultiTenantBlobStorage.Extensions;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.Default
{
    public class DefaultTenantContainerNameService : ITenantContainerNameService
    {
        public Task<string> GetContainerNameAsync(TenantRoute route)
        {
            if (route.Resource.IsMissing())
                return Task.FromResult<string>(null);
            return Task.FromResult(string.Format("{0}-{1}", new Guid(route.TenantId).ToString("N"), route.Resource));
        }
    }
}
