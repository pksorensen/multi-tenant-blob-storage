using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using SInnovations.Azure.MultiTenantBlobStorage.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace SInnovations.Azure.MultiTenantBlobStorage.Configuration
{
    public class MultiTenantBlobStorageServiceFactory
    {
        static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public Registration<ITenantContainerNameService> TenantContainerNameService { get; set; }
        public Registration<IStorageAccountResolverService> StorageAccountResolver { get; set; }
        public Registration<IResourceAuthorizationManager> AuthorizationManager { get; set; }
    }
}
