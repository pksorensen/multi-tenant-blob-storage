using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.Default.BlobStorage
{
    internal interface ICanonicalizer
    {
        string AuthorizationScheme { get; }

        string CanonicalizeHttpRequest(HttpRequestMessage request, string accountName);
    }
}
