using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Events.Base
{
    public class EventConstants
    {
        public static class Categories
        {

            public const string InternalError = "InternalError";
        }
        public static class Ids
        {
            private const int InternalErrorEventsStart = 5000;

            public const int UnhandledExceptionError = InternalErrorEventsStart + 0;
        }
    }
}
