using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Auth.Protocol;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Core;
using Microsoft.WindowsAzure.Storage.Core.Auth;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.Default
{
    public class DefaultBlobAuthenticationService : IBlobAuthenticationService
    {
        public CloudStorageAccount account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=ascendxyzweutest;AccountKey=nIKBpctAluX9ZYRbFHPcAq5g8HGMlreFdHVh/aSABJX6GJbVVoz4oELNljYA9S3knGBO8i6MxxfVwl4pmL+zKA==");
                
        public StorageCredentials SAS { get; set; }

        //public Task<Uri> UpdateRequestUri(Uri request, TenantRoute route)
        //{
        //    if (SAS == null)
        //    {

        //        SAS = new StorageCredentials(account.CreateCloudBlobClient().GetContainerReference(route.ContainerName).GetSharedAccessSignature(
        //            new SharedAccessBlobPolicy
        //            {
        //                Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Delete,

        //                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(1)
        //            }));
        //    };
        //    //request. = SAS.TransformUri(request.RequestUri);
        //    //request.RequestUri.q

        //    return Task.FromResult(0);

        //}

        public Task SignRequestAsync(HttpWebRequest request, TenantRoute route)
        {
            var a = new SharedKeyAuthenticationHandler(SharedKeyCanonicalizer.Instance, account.Credentials, account.Credentials.AccountName);
            a.SignRequest(request,null);
            return Task.FromResult(0);

        }
    }

       

        
   
  

}




