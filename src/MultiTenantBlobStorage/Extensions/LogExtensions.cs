using Microsoft.Owin;
using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Logging
{
    public static class LogRequestExtensions
    {
        public static void LogRequest(this ILog logger, WebRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(string.Format("[{0}] {1}", request.Method, request.RequestUri));
            foreach (var key in request.Headers.AllKeys)
            {
                sb.AppendLine(string.Format("{0}: {1}", key, string.Join(", ", request.Headers.GetValues(key))));
            }
            logger.Info(sb.ToString());
        }
        public static void LogResponse(this ILog logger, IOwinResponse response)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(string.Format("[{0}] {1}", response.StatusCode, response.ReasonPhrase));
            foreach (var header in response.Headers)
            {
                sb.AppendLine(string.Format("{0}: {1}", header.Key, string.Join(", ", header.Value)));
            }
            logger.Info(sb.ToString());
        }

        public static void LogRequest(this ILog logger, IOwinRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(string.Format("[{0}] {1}", request.Method, request.Uri));
            foreach (var header in request.Headers)
            {
                sb.AppendLine(string.Format("{0}: {1}", header.Key, string.Join(", ", header.Value)));
            }
            logger.Info(sb.ToString());
        }
        public static void LogRequest(this ILog logger, HttpRequestMessage request)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(string.Format("[{0}] {1}", request.Method, request.RequestUri));
            foreach (var header in request.Headers)
            {
                sb.AppendLine(string.Format("{0}: {1}", header.Key, string.Join(", ", header.Value)));
            }
            logger.Info(sb.ToString());
        }
    }
}
