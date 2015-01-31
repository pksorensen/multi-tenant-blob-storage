using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting
{
    internal class RequireSslMiddleware
    {
        readonly Func<IDictionary<string, object>, Task> _next;

        public RequireSslMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            var context = new OwinContext(env);

            if (context.Request.Uri.Scheme != Uri.UriSchemeHttps)
            {
                context.Response.StatusCode = 403;
                context.Response.ReasonPhrase = "ssl is requried";

                await context.Response.WriteAsync(context.Response.ReasonPhrase);

                return;
            }

            await _next(env);
        }
    }
}
