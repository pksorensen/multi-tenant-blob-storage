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
         
        public virtual CloudStorageAccount GetStorageAccount(string tenant, string purpose=null)
        {
            return Options.StorageAccountOptions.DefaultStorageAccount;
        }
        
        public virtual Task<string> GetBlobEndpointAsync(TenantRoute route)
        {
            
            return Task.FromResult(GetStorageAccount(route.TenantId).BlobEndpoint.AbsoluteUri);
        }

        public virtual Task SignRequestAsync(HttpWebRequest request, TenantRoute route)
        {
            var account = GetStorageAccount(route.TenantId,route.Purpose);
            var a = new SharedKeyAuthenticationHandler(SharedKeyCanonicalizer.Instance, account.Credentials, account.Credentials.AccountName);
            a.SignRequest(request, null);
            return Task.FromResult(0);

        }
    }
}
