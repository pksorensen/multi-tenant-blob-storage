using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SInnovations.Azure.MultiTenantBlobStorage.Configuration
{
    public class ReplacementOperation
    {
        public string Method{get;set;}
        public string Query{get;set;}
        public Func<Stream> Content{get;set;}
        public Action<IOwinResponse> Response { get; set; }
    }
    public class DeleteOptions
    {
        public Func<IOwinRequest, ReplacementOperation> OnDelete { get; set; }

        public Func<IDictionary<string, string>> SetMetaDataOnDelete { get; set; }
    }
}
