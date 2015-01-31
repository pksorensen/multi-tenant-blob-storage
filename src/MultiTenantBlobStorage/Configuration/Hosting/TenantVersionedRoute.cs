using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting
{
   
    public class TenantVersionedRoute : RouteFactoryAttribute
    {
        private static ILog Logger = LogProvider.GetCurrentClassLogger();

        public TenantVersionedRoute(string template, string allowedVersion)
            : base(template)
        {
            AllowedVersion = allowedVersion;
        }

        public string AllowedVersion
        {
            get;
            private set;
        }

        public override IDictionary<string, object> Constraints
        {
            get
            {
                var constraints = new HttpRouteValueDictionary();
                constraints.Add("version", new VersionConstraint(AllowedVersion));
                return constraints;
            }
        }
    }
}
