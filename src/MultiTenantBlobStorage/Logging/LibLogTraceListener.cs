using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Logging
{
    internal class LibLogTraceListener : TraceListener
    {
        private static readonly ILog Logger = LogProvider.GetLogger("WebApi Diagnostics");
        public override void WriteLine(string message)
        {
            Logger.Debug(message);
        }
        public override void Write(string message)
        { }
    }
}
