using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using SInnovations.Azure.MultiTenantBlobStorage.Services;
using SInnovations.Azure.MultiTenantBlobStorage.Services.RequestHandlers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SInnovations.Azure.MultiTenantBlobStorage.Extensions;
using SInnovations.Azure.MultiTenantBlobStorage.SasTokenExtension.Models;
using SInnovations.Azure.MultiTenantBlobStorage.Logging;

namespace SInnovations.Azure.MultiTenantBlobStorage.SasTokenExtension
{
    public class CompSasTokenHandler : IRequestHandler
    {
        private static ILog Logger = LogProvider.GetCurrentClassLogger();

        public const string BlobCompSas = Constants.Actions.BlobGet + "_sas";
        public const string ContainerCompSas = Constants.Actions.ContainerGet + "_sas";

        private readonly IStorageAccountResolverService _storage;
        private readonly ITenantContainerNameService _containers;
        private readonly ISharedAccessTokenService _tokenservice;
        public CompSasTokenHandler(IStorageAccountResolverService storage, ITenantContainerNameService containers, ISharedAccessTokenService tokenservice)
        {
            _storage = storage;
            _containers = containers;
            _tokenservice = tokenservice;
        }
        public Task<bool> CanHandleRequestAsync(IOwinContext context, ResourceContext resourceContext)
        {
            return Task.FromResult(resourceContext.Action == BlobCompSas || resourceContext.Action == ContainerCompSas);
        }
       
       
        public async Task<bool> OnBeforeHandleRequestAsync(IOwinContext context, ResourceContext resourceContext)
        {
            Logger.InfoFormat("Sas token generation begin for: {0}", resourceContext.Action);
           
            var model = await _tokenservice.GetTokenModelAsync(context, resourceContext);
            var token = string.Format("?token={0}", await _tokenservice.GetTokenAsync(model));

            Logger.InfoFormat("Sas token generation completed for: {0}", resourceContext.Action);
                      

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            using (var writer = new JsonTextWriter(new StreamWriter(context.Response.Body)))
            {

                new JObject(
                    new JProperty("sas",
                        token)
                        ).WriteTo(writer);

                writer.Flush();
            }


            return true;
        }

        public Task OnAfterHandleRequestAsync(Microsoft.Owin.IOwinContext context, Configuration.Hosting.ResourceContext resourceContext)
        {
            Logger.Warn("Entered not implemeted section");
            throw new NotImplementedException();
        }
    }
}
