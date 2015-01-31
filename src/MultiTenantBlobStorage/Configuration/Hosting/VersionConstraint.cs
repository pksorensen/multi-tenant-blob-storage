using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting
{
    /// <summary>
    /// A Constraint implementation that matches an HTTP header against an expected version value.
    /// </summary>
    public class VersionConstraint : IHttpRouteConstraint
    {
        public const string VersionHeaderName = "api-version";

        private const string DefaultVersion = Constants.ApiVersions.V1;

        public VersionConstraint(string allowedVersion)
        {
            AllowedVersion = allowedVersion;
        }

        public string AllowedVersion
        {
            get;
            private set;
        }

        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (routeDirection == HttpRouteDirection.UriResolution)
            {
                string version = GetVersionHeader(request) ?? DefaultVersion;
                if (version == AllowedVersion)
                {
                    return true;
                }
            }

            return false;
        }

        private string GetVersionHeader(HttpRequestMessage request)
        {
            string versionAsString = null;
            IEnumerable<string> headerValues;
            if (request.Headers.TryGetValues(VersionHeaderName, out headerValues) && headerValues.Count() == 1)
            {
                versionAsString = headerValues.First();
            }
            else
            {
                return null;
            }
            return versionAsString;
        }
    }
}
