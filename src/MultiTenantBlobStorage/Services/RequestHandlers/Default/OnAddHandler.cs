using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.RequestHandlers.Default
{
    public abstract class OnAddHandler : IRequestHandler
    {

        public Task<bool> CanHandleRequestAsync(Microsoft.Owin.IOwinContext context, Configuration.Hosting.ResourceContext resourceContext)
        {
            return Task.FromResult(resourceContext.Action == Constants.Actions.PutBlockList || resourceContext.Action == Constants.Actions.BlobPut);
        }
        
        public abstract Task<bool> OnBeforeHandleRequestAsync(Microsoft.Owin.IOwinContext context, Configuration.Hosting.ResourceContext resourceContext);


        public abstract Task OnAfterHandleRequestAsync(Microsoft.Owin.IOwinContext context, Configuration.Hosting.ResourceContext resourceContext);
        
    }
}
