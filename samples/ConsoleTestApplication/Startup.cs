using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration;
using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using SInnovations.Azure.MultiTenantBlobStorage;
using System.Text;
using Microsoft.WindowsAzure.Storage;

[assembly: OwinStartup(typeof(Host.Startup))]

namespace Host
{
    public class Startup
    {
        //static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            app.Map(Constants.OwinDefaultMapping, (builder) =>
            {
                //builder.Use(async (ctx, next) =>
                //{
                    
                    
                //    Logger.LogRequest(ctx.Request);
                    
                //    await next();

                //    Logger.LogResponse(ctx.Response);
                   


                //});

                builder.UseMultiTenantBlobStorage(new MultiTenantBlobStorageOptions
                {
                    ContainerResourceName = "workset",
                     DefaultStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=ascendxyzweutest;AccountKey=nIKBpctAluX9ZYRbFHPcAq5g8HGMlreFdHVh/aSABJX6GJbVVoz4oELNljYA9S3knGBO8i6MxxfVwl4pmL+zKA=="),

                    ListBlobOptions = new ListBlobOptions
                    {
                       
                        BlobListFilter = (x,state) =>
                       {
                           Console.WriteLine(x.ToString());
                           Console.WriteLine(x.Element("Properties").Element("BlobType").Value);
                           if (x.Element("Properties").Element("Content-Length").Value != "3")
                               return true;
                           return false;
                       },
                    }
                });
            });
        }
    }
}
