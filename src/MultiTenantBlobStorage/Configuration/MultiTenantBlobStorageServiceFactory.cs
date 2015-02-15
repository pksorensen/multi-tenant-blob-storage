using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using SInnovations.Azure.MultiTenantBlobStorage.Services;
using SInnovations.Azure.MultiTenantBlobStorage.Services.RequestHandlers;
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
   //     internal List<Registration<IRequestHandler>> Handlers = new List<Registration<IRequestHandler>>();
        internal List<Registration> Registrations = new List<Registration>();
        public Registration<ITenantContainerNameService> TenantContainerNameService { get; set; }
        public Registration<IStorageAccountResolverService> StorageAccountResolver { get; set; }
        public Registration<IResourceAuthorizationManager> AuthorizationManager { get; set; }

        public void RegisterHandler<T>(string name = null) where T : IRequestHandler
        {
            Registrations.Add(new Registration<IRequestHandler, T>(name ?? typeof(T).Name));
        }
        public void Register(Registration registration)
        {
            this.Registrations.Add(registration);
        }
    }
}
