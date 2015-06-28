using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SInnovations.Azure.MultiTenantBlobStorage.Extensions;

namespace Microsoft.WindowsAzure.Storage.Blob
{
    public static class StorageExtensions
    {
        public static async Task<bool> LoadTokensAsync(this CloudBlockBlob blob, List<string> tokens)
        {
            if (blob.Properties.ETag.IsMissing())
            {
                await blob.FetchAttributesAsync();
            }
            if (blob.Metadata.ContainsKey("token"))
            {
                var news = blob.Metadata["token"].Split(',').Where(token => tokens.Any(t => string.Equals(t, token, StringComparison.OrdinalIgnoreCase))).ToArray();


                tokens.AddRange(news);
                return true;
            }
            return false;
        }
        public static async Task SaveTokensAsync(this CloudBlockBlob blob, List<string> tokens)
        {
            blob.Metadata["token"] = string.Join(",", tokens);
            await blob.SetMetadataAsync();
        }
    }
}
