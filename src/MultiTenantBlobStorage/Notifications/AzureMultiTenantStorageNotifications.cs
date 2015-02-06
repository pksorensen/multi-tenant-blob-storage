using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Notifications
{
    public class AzureMultiTenantStorageNotifications
    {
        private static Task emptyTask = Task.FromResult(0);
        public AzureMultiTenantStorageNotifications()
        {
            
            Notification = (n) => emptyTask;
        }

        public Func<BaseNotification, Task> Notification { get; set; }
    }
}
