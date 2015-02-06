using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Notifications
{
    public class BaseNotification
    {
        public string Resource { get; set; }

        public string Tenant { get; set; }
        public bool CancelOperation { get; set; }
        
    }
}
