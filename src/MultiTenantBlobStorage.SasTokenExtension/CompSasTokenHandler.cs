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

namespace SInnovations.Azure.MultiTenantBlobStorage.SasTokenExtension
{
    public class CompSasTokenHandler : IRequestHandler
    {
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
          

            var account = _storage.GetStorageAccount(resourceContext.Route);
            var model = await _tokenservice.GetTokenModelAsync(context, resourceContext);

          
            if (model.Claims.Any(c=>c.Type=="token"))
            {               
                var container =  account.CreateCloudBlobClient()
                    .GetContainerReference(await _containers.GetContainerNameAsync(resourceContext.Route));

                var tokens = model.Claims.Where(k => k.Type == "token").Select(t => t.Value).ToList();

                if(resourceContext.Route.Path.IsPresent())
                {
                    var blob = container.GetBlockBlobReference(resourceContext.Route.Path);

                    await blob.FetchAttributesAsync();
                    if (blob.Metadata.ContainsKey("token"))
                        tokens.AddRange(blob.Metadata["token"].Split(','));

                    blob.Metadata["token"] = string.Join(",", tokens);
                    
                    await blob.SetMetadataAsync();
                }
                else
                {
                    await container.FetchAttributesAsync();
                    if (container.Metadata.ContainsKey("token"))
                        tokens.AddRange(container.Metadata["token"].Split(','));
                   
                    container.Metadata["token"] = string.Join(",", tokens);

                    await container.SetMetadataAsync();
                }
                
            }

            var token = string.Format("?token={0}", await _tokenservice.GetTokenAsync(model));
                      

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
            throw new NotImplementedException();
        }
    }
}
