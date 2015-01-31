using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage
{
    public class Constants
    {
        public static class ApiVersions
        {
            public const string V1 = "2015-01-01";
            public const string V2 = "2015-06-01";
        }

        public static class OwinContextProperties
        {
            public const string UnityRequestContainer = "mtbs:UnityRequestContainer";
            public const string Host = "mtbs:host";
            public const string BasePath = "mtbs:basePath";
            public const string RequestId = "mtbs:RequestId";
        }
        public static class OwinAppBuilderProperties
        {
            public const string UnityContainer = "mtbs:UnityContainer";
        }
    }
}
