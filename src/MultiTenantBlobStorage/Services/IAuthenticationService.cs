using Microsoft.Owin;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Finds the user for the current request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<ClaimsPrincipal> AuthenticateRequestAsync(IOwinRequest request, MultiTenantBlobStorageOptions options);

        Task<bool> SkipAuthorizationManagerAsync(OwinContext context, ResourceContext resourceContext);
    }
}
