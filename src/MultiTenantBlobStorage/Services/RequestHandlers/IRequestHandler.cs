using Microsoft.Owin;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.RequestHandlers
{
    public interface IRequestHandler
    {
        Task<bool> CanHandleRequestAsync(IOwinContext context, ResourceContext resourceContext);
        Task<Boolean> OnBeforeHandleRequestAsync(IOwinContext context, ResourceContext resourceContext);
        Task OnAfterHandleRequestAsync(IOwinContext context, ResourceContext resourceContext);
    }
    public interface IRequestHandler<T> : IRequestHandler where T : RequestOptions
    {
        bool ImplementsRequestTransformer { get; }
        bool ImplementsHandleRequest { get; }

        Task Transformer(Stream incoming, Stream outgoing, T options);

        Task HandleRequest(IOwinContext context, ResourceContext resourceContext, T options);

       
    }
}
