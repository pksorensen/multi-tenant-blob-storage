using Microsoft.Owin;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services
{
    public interface IRequestHandlerService
    {

     
        /// <summary>
        /// Finds the user for the current request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<ClaimsPrincipal> AuthenticateRequestAsync(IOwinRequest request, MultiTenantBlobStorageOptions options);

        /// <summary>
        /// Parse the request route data
        /// </summary>
        /// <param name="request"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<TenantRoute> ParseRouteDataAsync(IOwinRequest request, MultiTenantBlobStorageOptions options);

        /// <summary>
        /// Handles the request
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resourceContext"></param>
        /// <returns></returns>
        Task HandleAsync(IOwinContext context, ResourceContext resourceContext);
    }
}
