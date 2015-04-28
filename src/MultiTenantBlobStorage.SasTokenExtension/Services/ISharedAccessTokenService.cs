using Microsoft.Owin;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using SInnovations.Azure.MultiTenantBlobStorage.SasTokenExtension.Models;
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
        Task<SasTokenGenerationModel> GetTokenModelAsync(IOwinContext context, ResourceContext resourceContext);
        Task<string> GetTokenAsync(SasTokenGenerationModel model);

        Task<IEnumerable<Claim>> CheckSignatureAsync(string token);
    }
}
