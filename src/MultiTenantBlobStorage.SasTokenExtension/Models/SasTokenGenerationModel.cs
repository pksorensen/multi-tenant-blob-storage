using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SInnovations.Azure.MultiTenantBlobStorage.Extensions;

namespace SInnovations.Azure.MultiTenantBlobStorage.SasTokenExtension.Models
{
    public class SasTokenGenerationModel
    {
        public SasTokenGenerationModel()
        {
            Claims = new List<Claim>();
        }
        public SasTokenGenerationModel(string tenant,string purpose, string resource,string path = null)
        {
            Claims = new List<Claim> {
                new Claim("tenant",tenant),
                new Claim("resource",resource),
                new Claim("purpose",purpose),            
            };
            if (path.IsPresent())
                Claims.Add(new Claim("prefix", path));
        }
        //public DateTimeOffset? Expires { get; set; }
        public List<Claim> Claims { get; set; }
    }

    public static class SasTokenGenerationModelExtensions
    {
        public static SasTokenGenerationModel ExpireAt(this SasTokenGenerationModel model, DateTimeOffset expire)
        {
            model.Claims.Add(new Claim("exp", expire.ToString("R", CultureInfo.InvariantCulture)));

            return model;
        }
        public static SasTokenGenerationModel WithReadAccess(this SasTokenGenerationModel model)
        {
            model.Claims.Add(new Claim("role", "read"));

            return model;
        }
        public static SasTokenGenerationModel WithWriteAccess(this SasTokenGenerationModel model)
        {
            model.Claims.Add(new Claim("role", "write"));

            return model;
        }
    }
}
