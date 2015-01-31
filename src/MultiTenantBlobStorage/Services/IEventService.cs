using SInnovations.Azure.MultiTenantBlobStorage.Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services
{
    /// <summary>
    /// Models a recipient of notification of events
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <param name="evt">The event.</param>
        void Raise<T>(Event<T> evt);
    }
}
