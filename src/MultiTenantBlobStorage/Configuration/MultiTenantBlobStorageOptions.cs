using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
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
    
    public class Blob{
        public string Name {get;set;}
        public IDictionary<string, string> Properties { get; set; }
        public IDictionary<string, string> MetaData { get; set; }
    }
    public class ListBlobOptions
    {
        /// <summary>
        /// Function is invoked on all blobs returned 
        /// </summary>
        public Func<XElement, bool> BlobListFilter { get; set; }


        public Func<Task<IEnumerable<Blob>>> BlobListProvider { get; set; }

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
            DeleteOptions = new DeleteOptions();

          //  var blob = new CloudBlockBlob(new Uri(""));
           //var a = new BlobProperties() { BlobType = "aa"}; 
        }

        public MultiTenantBlobStorageServiceFactory Factory { get; set; }
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

        public DeleteOptions DeleteOptions { get; set; } 

        public CloudStorageAccount DefaultStorageAccount { get; set; }
    }
}
