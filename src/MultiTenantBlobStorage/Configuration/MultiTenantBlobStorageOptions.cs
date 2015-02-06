using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using SInnovations.Azure.MultiTenantBlobStorage.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SInnovations.Azure.MultiTenantBlobStorage.Configuration
{
    public class ListBlobOptions
    {
        /// <summary>
        /// Function is invoked on all blobs returned 
        /// </summary>
        public Func<XElement, bool> BlobListFilter { get; set; }

        

       // public 
    }
    public class MultiTenantBlobStorageOptions
    {
        static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public MultiTenantBlobStorageOptions()
        {
            //ContainerResourceName = "Container";
            LoggingOptions = new LoggingOptions();
            Notifications = new AzureMultiTenantStorageNotifications();
            ListBlobOptions = new ListBlobOptions();
        }

        public AzureMultiTenantStorageNotifications Notifications { get; set; }

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


        public string AuthenticationType { get; set; }

        public ListBlobOptions ListBlobOptions { get; set; }

       
    }
}
