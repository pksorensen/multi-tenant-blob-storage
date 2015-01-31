using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Logging
{
    internal class RequestResponseLogger : DelegatingHandler
    {
        static readonly ILog Logger = LogProvider.GetLogger("HTTP Logging");
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var reqLog = new
            {
                Method = request.Method,
                Url = request.RequestUri.AbsoluteUri,
                Headers = request.Headers,
                Body = await request.Content.ReadAsStringAsync()
            };
            Logger.DebugFormat("HTTP Request\n{0}", LogSerializer.Serialize(reqLog));
            var response = await base.SendAsync(request, cancellationToken);
            string body = "";
            if (response.Content != null)
            {
                body = await response.Content.ReadAsStringAsync();
            }
            var respLog = new
            {
                StatusCode = response.StatusCode,
                Headers = response.Headers,
                Body = body
            };
            Logger.DebugFormat("HTTP Response\n{0}", LogSerializer.Serialize(respLog));
            return response;
        }
    }
}
