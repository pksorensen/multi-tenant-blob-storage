using SInnovations.Azure.MultiTenantBlobStorage.Events.Base;
using SInnovations.Azure.MultiTenantBlobStorage.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Extensions
{
    internal static class IEventServiceExtensions
    {
        public static void RaiseUnhandledExceptionEvent(this IEventService events, Exception exception)
        {
            var evt = new Event<object>(
                EventConstants.Categories.InternalError,
                "Unhandled exception",
                EventTypes.Error,
                EventConstants.Ids.UnhandledExceptionError,
                exception.ToString());

            events.RaiseEvent(evt);
        }

        private static void RaiseEvent<T>(this IEventService events, Event<T> evt)
        {
            if (events == null) throw new ArgumentNullException("events");

            events.Raise(evt);
        }
    }
}
