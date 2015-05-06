using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Extensions
{
    public static class ClaimExtensions
    {
        public static string FindFirstOrEmptyValue(this IEnumerable<Claim> claims, string type)
        {
            var prefix = claims.FirstOrDefault(c => c.Type == type);
            if (prefix == null)
                return "";
            return prefix.Value;
        }
    }
}
