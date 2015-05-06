using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services
{
    public class TenantRoute
    {
        public string TenantId { get; set; }
        public string Purpose { get; set; }
        public string Resource { get; set; }
        public string ContainerName { get; set; }

        public string Path { get; set; }
        public string Host { get; set; }
    }
    public interface IRequestTenantResolver
    {
        Task<TenantRoute> GetRouteAsync(Microsoft.Owin.IOwinRequest owinRequest);
    }
}
