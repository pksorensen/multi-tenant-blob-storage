using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services
{
    public interface IBlobAuthenticationService
    {
       
      

        Task SignRequestAsync(HttpWebRequest request, TenantRoute tenantRoute);
    }
}
