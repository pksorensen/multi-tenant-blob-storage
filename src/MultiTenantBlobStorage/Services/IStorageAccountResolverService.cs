using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services
{
    public interface IStorageAccountResolverService
    {
        Task<CloudStorageAccount> GetStorageAccountAsync(string tenant, string purpose = null);
        Task SignRequestAsync(HttpWebRequest request, TenantRoute tenantRoute);
        Task<string> GetBlobEndpointAsync(TenantRoute tenantRoute);
    }
}
