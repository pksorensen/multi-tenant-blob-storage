using Microsoft.Owin;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.Default
{
    public class DefaultAuthenticationService : IAuthenticationService
    {

        public virtual async Task<ClaimsPrincipal> AuthenticateRequestAsync(IOwinRequest request, MultiTenantBlobStorageOptions options)
        {
            var result = await request.Context.Authentication.AuthenticateAsync(options.AuthenticationType);
            if (result == null)
                return null;

            return new ClaimsPrincipal(result.Identity);
        }


        public virtual Task<bool> SkipAuthorizationManagerAsync(OwinContext context, Configuration.Hosting.ResourceContext resourceContext)
        {
            return Task.FromResult(false);
        }


        public virtual Task<bool> BlobStorageResponseAuthorizedAsync(IOwinContext context, ResourceContext resourceContext, System.Net.HttpWebResponse response)
        {
            return Task.FromResult(true);
        }
    }
}
