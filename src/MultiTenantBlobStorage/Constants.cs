using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage
{
    public class Constants
    {
        public static class Actions
        {
            public const string ContainerPrefix = "container";
            public const string ListBlobs   = ContainerPrefix+"_get";
            public const string ContainerExists = ContainerPrefix + "_head";
            public const string ContainerCreate = ContainerPrefix + "_put";
            public const string ContainerDelete = ContainerPrefix + "_delete";
            public const string BlobPrefix = "blob";
            public const string BlobPut = BlobPrefix + "_put";
        }
        public static class ContentTypes
        {
            public const string Xml = "application/xml";
            public const string Json = "application/json";
        }
        public static class HeaderConstants
        {
            /// <summary>
            /// Master Windows Azure Storage header prefix.
            /// </summary>
            public const string PrefixForStorageHeader = "x-ms-";
          
            /// <summary>
            /// Header that specifies the key name for explicit keys.
            /// </summary>
            public const string KeyNameHeader = PrefixForStorageHeader + "key-name";

            public const string UserAgent = "user-agent";
            public const string Accept = "accept";
            public const string Connection = "connection";
            public const string ContentLenght = "content-length";
            public const string ContentType = "content-type";
            public const string Date = "date";
            public const string Expect = "expect";
            public const string Host = "host";
            public const string IfModifiedSince = "if-modified-since";
            public const string Range = "range";
            public const string Referer = "referer";
            public const string TansferEncoding = "transfer-encoding";
            public const string ProxyConnection = "proxy-connection";

        }
        public const string OwinDefaultMapping = "/files";
        public static class ApiVersions
        {
            public const string V1 = "2015-01-01";
            public const string V2 = "2015-06-01";
        }

        public static class HttpMethods
        {
            public const string Get = "GET";
            public const string Delete = "DELETE";
            public const string Put = "PUT";
            public const string Post = "POST";
            public const string Head = "HEAD";
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
