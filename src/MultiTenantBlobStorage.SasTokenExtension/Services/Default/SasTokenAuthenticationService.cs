using Microsoft.Owin;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using SInnovations.Azure.MultiTenantBlobStorage.Services;
using SInnovations.Azure.MultiTenantBlobStorage.Services.Default;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.SasTokenExtension.Services.Default
{
    public class SasTokenAuthenticationService : DefaultAuthenticationService
    {
        private readonly ISharedAccessTokenService _tokenService;
        public SasTokenAuthenticationService(ISharedAccessTokenService tokenService)
        {
            this._tokenService = tokenService;
        }
        public override bool SkipAuthorizationManager(OwinContext context, ResourceContext resourceContext)
        {

            var token = context.Request.Query["token"];
            IEnumerable<Claim> claims;
            if (!string.IsNullOrWhiteSpace(token) && _tokenService.CheckSignature(token,out claims))
            {
                var prefix = claims.First(c=>c.Type=="prefix").Value;
                var exp = claims.First(c=>c.Type=="exp");
                var flag = resourceContext.Route.Path.StartsWith(prefix);
                var expired = DateTimeOffset.UtcNow >= DateTimeOffset.Parse(exp.Value, CultureInfo.InvariantCulture);

                return flag && !expired;
            }

            return base.SkipAuthorizationManager(context,resourceContext);
        }
    }
}
