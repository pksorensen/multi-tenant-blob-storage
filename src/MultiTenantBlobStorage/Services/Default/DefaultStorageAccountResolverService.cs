using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth.Protocol;
using Microsoft.WindowsAzure.Storage.Core.Auth;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.Default
{
    public class DefaultStorageAccountResolverService : IStorageAccountResolverService
    {
        protected MultiTenantBlobStorageOptions Options { get; set; } 
        public DefaultStorageAccountResolverService(MultiTenantBlobStorageOptions options)
        {
            Options = options;
        }
         
        public virtual Task<CloudStorageAccount> GetStorageAccountAsync(string tenant, string purpose=null)
        {
            return Task.FromResult(Options.StorageAccountOptions.DefaultStorageAccount);
        }
        
        public virtual async Task<string> GetBlobEndpointAsync(TenantRoute route)
        {
            var account = await GetStorageAccountAsync(route.TenantId,route.Purpose);
            return account.BlobEndpoint.AbsoluteUri;
        }

        public virtual async Task SignRequestAsync(HttpWebRequest request, TenantRoute route)
        {
            var account = await GetStorageAccountAsync(route.TenantId,route.Purpose);
            var a = new SharedKeyAuthenticationHandler(SharedKeyCanonicalizer.Instance, account.Credentials, account.Credentials.AccountName);
            a.SignRequest(request, null);
           
        }
    }
}
