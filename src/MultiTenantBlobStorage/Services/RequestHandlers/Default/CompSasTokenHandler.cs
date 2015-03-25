using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.RequestHandlers.Default
{
    public class CompSasTokenHandler : IRequestHandler
    {
        public const string BlobCompSas = Constants.Actions.BlobGet + "_sas";
        public const string ContainerCompSas = Constants.Actions.ContainerGet + "_sas";

        private readonly IStorageAccountResolverService _storage;
        private readonly ITenantContainerNameService _containers;
        public CompSasTokenHandler(IStorageAccountResolverService storage, ITenantContainerNameService containers)
        {
            _storage = storage;
            _containers = containers;
        }
        public Task<bool> CanHandleRequestAsync(Microsoft.Owin.IOwinContext context, ResourceContext resourceContext)
        {
            return Task.FromResult(resourceContext.Action == BlobCompSas || resourceContext.Action == ContainerCompSas);
        }

        public async Task<bool> OnBeforeHandleRequestAsync(Microsoft.Owin.IOwinContext context, ResourceContext resourceContext)
        {
            var expireTIme = DateTimeOffset.UtcNow.AddHours(4);
            
            String dateInRfc1123Format = expireTIme.ToString("R", CultureInfo.InvariantCulture);
            
            var account = _storage.GetStorageAccount(resourceContext.Route);
            var policy = new SharedAccessBlobPolicy
                            { 
                                Permissions = SharedAccessBlobPermissions.Read, 
                                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(4) 
                            };
            string signature = "";
            var ac = "rw";
            using (HMACSHA256 hmacSha256 = new HMACSHA256(Convert.FromBase64String(account.Credentials.ExportBase64EncodedKey())))
            {
               
                var sb = new StringBuilder();
                sb.AppendLine(ac);
                sb.AppendLine(resourceContext.Route.TenantId);
                sb.AppendLine(resourceContext.Route.Resource);
                if (!string.IsNullOrWhiteSpace(resourceContext.Route.Path))
                {
                    sb.AppendLine(resourceContext.Route.Path);
                }
                sb.AppendLine(dateInRfc1123Format);


                Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
               
            }

            var sas = string.Format("?s={0}&e={1}&ac={2}&a={3}", signature, dateInRfc1123Format, ac,!string.IsNullOrWhiteSpace(resourceContext.Route.Path) ? "b":"c");

          
            //if(!string.IsNullOrWhiteSpace( resourceContext.Route.Path))
            //    sas = account.CreateCloudBlobClient()
            //    .GetContainerReference(await _containers.GetContainerNameAsync(resourceContext.Route))
            //    .GetBlockBlobReference(resourceContext.Route.Path).GetSharedAccessSignature(policy);
            //else{
            //      sas = account.CreateCloudBlobClient()
            //    .GetContainerReference(await _containers.GetContainerNameAsync(resourceContext.Route))
            //    .GetSharedAccessSignature(policy);
            //}


            

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            
            using(var writer = new JsonTextWriter(new StreamWriter(context.Response.Body)))
            {

                new JObject(
                    new JProperty("sas", 
                        sas),
                    new JProperty("expire", dateInRfc1123Format)
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
