using Microsoft.Owin;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services
{
    public interface ISharedAccessTokenService
    {
        IEnumerable<Claim> GetClaimsForToken(IOwinContext context, ResourceContext resourceContext);
        string GetToken(IEnumerable<Claim> claims);

        bool CheckSignature(string token, out IEnumerable<Claim> claims);
    }
}
