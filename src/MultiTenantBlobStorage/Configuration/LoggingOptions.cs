using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SInnovations.Azure.MultiTenantBlobStorage.Configuration
{
    /// <summary>
    /// Configures logging within IdentityServer.
    /// </summary>
    public class LoggingOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingOptions"/> class.
        /// </summary>
        public LoggingOptions()
        {
            EnableWebApiDiagnostics = false;
            WebApiDiagnosticsIsVerbose = false;
            EnableHttpLogging = false;
            IncludeSensitiveDataInLogs = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether web API diagnostics should be enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if web API diagnostics should be enabled; otherwise, <c>false</c>.
        /// </value>
        public bool EnableWebApiDiagnostics { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether web API diagnostics logging should be set to verbose.
        /// </summary>
        /// <value>
        /// <c>true</c> if web API diagnostics logging shozld be verbose; otherwise, <c>false</c>.
        /// </value>
        public bool WebApiDiagnosticsIsVerbose { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HTTP request/response logging is enabled
        /// </summary>
        /// <value>
        ///   <c>true</c> if HTTP logging is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool EnableHttpLogging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include (potentially) personally identifiable information or other sensitive data in logs.
        /// </summary>
        /// <value>
        ///   <c>true</c> if PII data should be included in logs; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeSensitiveDataInLogs { get; set; }
    }
}
