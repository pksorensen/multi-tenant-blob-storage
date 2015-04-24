using SInnovations.Azure.MultiTenantBlobStorage.Configuration;
using SInnovations.Azure.MultiTenantBlobStorage.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.SasTokenExtension.Extensions
{
    public static class MultiTenantBlobStorageServiceFactoryExtensions
    {
        public static void UseSasTokens(this MultiTenantBlobStorageServiceFactory factory, Func<IDependencyResolver,ISharedAccessTokenService> sharedAccessTokenServiceFactory  )
        {

            factory.Register(new Registration<ISharedAccessTokenService>(sharedAccessTokenServiceFactory));
            factory.RegisterHandler<CompSasTokenHandler>();

        }
    }
}
