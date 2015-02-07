using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting
{
    public class RequestOptions
    {
        public RequestOptions(IOwinRequest owinRequest)
        {
            // TODO: Complete member initialization
            ContentType = owinRequest.ContentType ?? Constants.ContentTypes.Xml;
            CallCancelled = owinRequest.CallCancelled;
        }
        public string ContentType { get; set; }
        public CancellationToken CallCancelled { get; set; }

    }
}
