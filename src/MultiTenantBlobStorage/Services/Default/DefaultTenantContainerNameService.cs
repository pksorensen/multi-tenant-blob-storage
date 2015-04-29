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
        public Task<string> GetContainerNameAsync(string tenant, string resource)
        {
            if (resource.IsMissing())
                return Task.FromResult<string>(new Guid(tenant).ToString("N"));
            return Task.FromResult(string.Format("{0}-{1}", new Guid(tenant).ToString("N"), resource));
        }
    }
}
