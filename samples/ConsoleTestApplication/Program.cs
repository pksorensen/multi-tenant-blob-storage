using Host;
using Microsoft.Owin.Hosting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using SInnovations.Azure.MultiTenantBlobStorage;
using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using SInnovations.Azure.MultiTenantBlobStorage.Extensions;

namespace ConsoleTestApplication
{

    class Program
    {
        
        public static bool Transform(XElement el)
        {
      
            el.Name = "a";

            return true;
        }
        static void Main(string[] args)
        {
         
            //var guid = Guid.NewGuid();
            LogProvider.SetCurrentLogProvider(new DiagnosticsTraceLogProvider());
            //Trace.Listeners.Add(new ConsoleTraceListener());

            var localhost = "http://localhost:46311";
            using (WebApp.Start<Startup>(localhost))
            {
                Console.WriteLine("Started...");
                var workset = new CloudBlobContainer(new Uri(localhost + Constants.OwinDefaultMapping + string.Format("/{0}/{1}", Guid.NewGuid(), "workset")));
                var workset1 = new CloudBlobContainer(new Uri(localhost + Constants.OwinDefaultMapping + string.Format("/{0}/{1}", Guid.NewGuid(), "workset")));

                var service = new CloudBlobClient(new Uri(localhost + Constants.OwinDefaultMapping + string.Format("/{0}/", new Guid())));

              var a =  service.ListContainers().ToList();

                // var msg = new HttpClient().GetAsync(workset.Uri.AbsoluteUri).GetAwaiter().GetResult();
                // Console.WriteLine(msg.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                if (false)
                {
                    WriteLine("workset.Exists()");
                    workset.Exists();

                    WriteLine("workset.Create()");
                    workset.Create();


                    WriteLine("workset.CreateIfNotExists()");
                    workset.CreateIfNotExists();


                    WriteLine("workset1.CreateIfNotExists()");
                    workset1.CreateIfNotExists();

                    WriteLine("workset1.Delete()");
                    workset1.Delete();

                    WriteLine("workset.DeleteIfExists()");
                    workset1.DeleteIfExists();

                    WriteLine("workset.ListBlobs()");
                    foreach (var blob in workset.ListBlobs(useFlatBlobListing:true))
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(blob, Newtonsoft.Json.Formatting.Indented));
                        return;
                    }
                    workset.DeleteIfExists();
                    WriteLine("DONE");

                }
                var workset3 = new CloudBlobContainer(new Uri(localhost + Constants.OwinDefaultMapping + string.Format("/{0}/{1}", new Guid(), "workset")));

                //workset3.CreateIfNotExists();
                //workset3.GetBlockBlobReference(Guid.NewGuid() + ".txt").UploadText("Hej");



              


                WriteLine("workset.ListBlobs()");
                foreach (var blob in workset3.ListBlobs(useFlatBlobListing:false))
                {
                 //   Console.WriteLine(JsonConvert.SerializeObject(blob, Newtonsoft.Json.Formatting.Indented));
                    //Console.ReadKey();
                }
                WriteLine("workset.ListBlobs() json");
                



                Console.ReadKey();

            }


        }
        public static void WriteLine(string header = null)
        {
            Console.WriteLine("-{0}{1}", header.IsPresent() ? " " + header + " " : "",
                string.Join("", Enumerable.Range(0, 80 - (header.IsPresent() ? header.Length + 2 : 0)).Select(k => "-")));
        }
    }
}
