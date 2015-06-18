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
        /// Handles the request
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resourceContext"></param>
        /// <returns></returns>
        Task HandleAsync(IOwinContext context, ResourceContext resourceContext);
    }
}
