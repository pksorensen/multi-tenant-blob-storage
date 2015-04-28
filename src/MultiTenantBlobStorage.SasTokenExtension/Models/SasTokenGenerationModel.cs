using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.SasTokenExtension.Models
{
    public class SasTokenGenerationModel
    {
        public SasTokenGenerationModel()
        {
            Claims = new List<Claim>();
        }
        public DateTimeOffset? Expires { get; set; }
        public List<Claim> Claims { get; set; }
    }
}
