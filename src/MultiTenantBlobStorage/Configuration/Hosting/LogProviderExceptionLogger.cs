using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using SInnovations.Azure.MultiTenantBlobStorage.Extensions;
using SInnovations.Azure.MultiTenantBlobStorage.Services;

namespace SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting
{
    internal class LogProviderExceptionLogger : IExceptionLogger
    {
        private readonly static ILog Logger = LogProvider.GetCurrentClassLogger();

        public Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            Logger.ErrorException("Unhandled exception", context.Exception);

            //var env = context.Request.GetOwinEnvironment();
            //var events = env.ResolveDependency<IEventService>();
            //events.RaiseUnhandledExceptionEvent(context.Exception);

            return Task.FromResult<object>(null);
        }
    }
}
