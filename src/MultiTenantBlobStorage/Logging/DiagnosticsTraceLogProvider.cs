using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Logging
{
    /// <summary>
    /// Implementation of <see cref="ILogProvider"/> that uses the <see cref="DiagnosticsTraceLogger"/>.
    /// </summary>
    public class DiagnosticsTraceLogProvider : ILogProvider
    {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public ILog GetLogger(string name)
        {
            return new DiagnosticsTraceLogger(name);
        }


        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Implementation of <see cref="ILog"/> that uses <see cref="System.Diagnostics.Trace"/>.
    /// </summary>
    public class DiagnosticsTraceLogger : ILog
    {
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsTraceLogger"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DiagnosticsTraceLogger(string name)
        {
            _name = string.Format("[{0}]", name);
        }


        /// <summary>
        /// Log a message and exception at the specified log level.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logLevel">The log level.</param>
        /// <param name="messageFunc">The message function.</param>
        /// <param name="exception">The exception.</param>
        /// <remarks>
        /// Note to implementors: the message func should not be called if the loglevel is not enabled
        /// so as not to incur perfomance penalties.
        /// </remarks>
        public void Log<TException>(LogLevel logLevel, Func<string> messageFunc, TException exception) where TException : Exception
        {
            if (messageFunc != null && exception != null)
            {
                var message = string.Format("{0}: {1} -- {2}\n{3}", _name, DateTimeOffset.UtcNow.ToString(), messageFunc(), exception.ToString());
                TraceMsg(logLevel, message);
            }
        }

        private static void TraceMsg(LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    Trace.WriteLine(message, logLevel.ToString());
                    break;
                case LogLevel.Info:
                    Trace.TraceInformation(message);
                    break;
                case LogLevel.Warn:
                    Trace.TraceWarning(message);
                    break;
                case LogLevel.Error:
                    Trace.TraceError(message);
                    break;
                case LogLevel.Fatal:
                    Trace.TraceError(string.Format("FATAL : {0}", message));
                    break;
            }
        }

        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
        {
            if (messageFunc != null)
            {
                if (exception == null)
                {
                    var message = string.Format("{0}: {1} -- {2}", _name, DateTimeOffset.UtcNow, messageFunc());
                    TraceMsg(logLevel, string.Format(message, formatParameters));
                }
                else
                {
                    var message = string.Format("{0}: {1} -- {2}\n{3}", _name, DateTimeOffset.UtcNow, messageFunc(), exception);
                    TraceMsg(logLevel, string.Format(message, formatParameters));
                }
            }

            return true;
        }
    }
}
