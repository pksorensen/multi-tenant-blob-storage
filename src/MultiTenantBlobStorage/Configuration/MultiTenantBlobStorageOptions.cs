using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Configuration
{
    public class MultiTenantBlobStorageOptions
    {
        static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public MultiTenantBlobStorageOptions()
        {
            ContainerResourceName = "Container";
        }

        /// <summary>
        /// In all endpoints the container resource can be replaced with a custom one.
        /// </summary>
        public string ContainerResourceName { get; set; }

        public bool RequireSsl { get; set; }

        /// <summary>
        /// Gets or sets the diagnostics options.
        /// </summary>
        /// <value>
        /// The diagnostics options.
        /// </value>
        public LoggingOptions LoggingOptions { get; set; }

    }
}
