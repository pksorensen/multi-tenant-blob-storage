using Microsoft.Owin;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using SInnovations.Azure.MultiTenantBlobStorage.Extensions;
using SInnovations.Azure.MultiTenantBlobStorage.Services;
using SInnovations.Azure.MultiTenantBlobStorage.Services.Default;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.SasTokenExtension.Services.Default
{
    public class SasTokenAuthenticationService : DefaultAuthenticationService
    {
        private readonly ISharedAccessTokenService _tokenService;
        private readonly IStorageAccountResolverService _storage;
        private readonly ITenantContainerNameService _containers;

    
        public SasTokenAuthenticationService(
            ISharedAccessTokenService tokenService, 
            IStorageAccountResolverService storage,
            ITenantContainerNameService containers
            )
        {
            this._tokenService = tokenService;
            this._storage = storage;
            this._containers = containers;

            RevokeTokens = new List<String>();
        }
        protected IEnumerable<String> RevokeTokens { get; set; }
        public override Task<bool> BlobStorageResponseAuthorizedAsync(IOwinContext context, ResourceContext resourceContext, System.Net.HttpWebResponse response)
        {

            if (RevokeTokens.Any())
            {
                var tokens = response.GetResponseHeader("x-ms-meta-token");
                return Task.FromResult(RevokeTokens.All(t => tokens.IndexOf(t) > -1));
            }
                
            return base.BlobStorageResponseAuthorizedAsync(context,resourceContext,response);
        }
        public override async Task<bool> SkipAuthorizationManagerAsync(OwinContext context, ResourceContext resourceContext)
        {

            var token = context.Request.Query["token"] ?? context.Request.Query["access_token"];
            var flag = true;

            if (!string.IsNullOrWhiteSpace(token))
            {
                var claims = await _tokenService.CheckSignatureAsync(token);
                if (!claims.Any())
                    return false;
                
                var prefix = GetValue(claims,"prefix");
                var resource = GetValue(claims, "resource");
                var tenant = GetValue(claims, "tenant");

                flag &= resourceContext.Route.Path.StartsWith(prefix);

                if (claims.Any(c=>c.Type=="exp"))
                {
                   
                    var exp = claims.First(c => c.Type == "exp");
                    
                    var expired = DateTimeOffset.UtcNow >= DateTimeOffset.Parse(exp.Value, CultureInfo.InvariantCulture);

                    flag &= !expired;
                }

                 var tokenids = claims.Where(t => t.Type == "token").ToArray();
                 if (tokenids.Any())
                 {
                     var tokens = await GetTokenIdsAsync(prefix, resource, tenant);
                     RevokeTokens = tokenids.Where(t => tokens.IndexOf(t.Value) == -1).Select(t => t.Value).ToArray();

                 }
                 return flag;
            }

          return false;

        }

        protected virtual async Task<string> GetTokenIdsAsync(string prefix, string resource, string tenant)
        {
            IDictionary<string, string> md;

            var blobClient = _storage.GetStorageAccount(tenant).CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(await _containers.GetContainerNameAsync(tenant, resource));

            if (prefix.IsPresent())
            {
                var blob = blobContainer.GetBlockBlobReference(prefix);
                await blob.FetchAttributesAsync();
                md = blob.Metadata;

            }
            else
            {
                await blobContainer.FetchAttributesAsync();
                md = blobContainer.Metadata;


            }
            string tokens;
            md.TryGetValue("token", out tokens);
            return tokens;
        }

       

        private static string GetValue(IEnumerable<Claim> claims,string type)
        {
            var prefix = claims.FirstOrDefault(c => c.Type ==type);
            if (prefix == null)
                return "";
            return prefix.Value;
        }
    }
}
