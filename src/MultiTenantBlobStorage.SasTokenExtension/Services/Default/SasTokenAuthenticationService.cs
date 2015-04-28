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
using System.Linq;

namespace SInnovations.Azure.MultiTenantBlobStorage.SasTokenExtension.Services.Default
{
    public class SasTokenAuthenticationService : DefaultAuthenticationService
    {
        private readonly ISharedAccessTokenService _tokenService;
        public SasTokenAuthenticationService(ISharedAccessTokenService tokenService)
        {
            this._tokenService = tokenService;
            Claims = new List<Claim>();
        }
        protected IEnumerable<Claim> Claims { get; set; }
        public override Task<bool> BlobStorageResponseAuthorizedAsync(IOwinContext context, ResourceContext resourceContext, System.Net.HttpWebResponse response)
        {
            var tokenids = Claims.Where(t => t.Type == "token").ToArray();
            if(tokenids.Any())
            {
                var tokens = response.GetResponseHeader("x-ms-meta-token");
                return Task.FromResult(tokenids.All(t => tokens.IndexOf(t.Value) > -1));
            }
                
            return base.BlobStorageResponseAuthorizedAsync(context,resourceContext,response);
        }
        public override async Task<bool> SkipAuthorizationManagerAsync(OwinContext context, ResourceContext resourceContext)
        {

            var token = context.Request.Query["token"];
           
            if (!string.IsNullOrWhiteSpace(token))
            {
                Claims = await _tokenService.CheckSignatureAsync(token);
                if (Claims.Any(c=>c.Type=="exp"))
                {
                    var prefix = GetPrefixValue(Claims);
                    var exp = Claims.First(c => c.Type == "exp");
                    var flag = resourceContext.Route.Path.StartsWith(prefix);
                    var expired = DateTimeOffset.UtcNow >= DateTimeOffset.Parse(exp.Value, CultureInfo.InvariantCulture);

                    return flag && !expired;
                }
            }

            return false;
        }

        private static string GetPrefixValue(IEnumerable<Claim> claims)
        {
            var prefix = claims.FirstOrDefault(c => c.Type == "prefix");
            if (prefix == null)
                return "";
            return prefix.Value;
        }
    }
}
