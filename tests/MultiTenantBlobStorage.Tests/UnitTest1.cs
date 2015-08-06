using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SInnovations.Azure.MultiTenantBlobStorage.Services.Default;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using SInnovations.Azure.MultiTenantBlobStorage.SasTokenExtension.Models;
using System.Xml;
using System.IO;
using SInnovations.Azure.MultiTenantBlobStorage.Services;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration;
using System.Text.RegularExpressions;

namespace MultiTenantBlobStorage.Tests
{
    [TestClass]
    public class UnitTest1
    {
        public static string GetRandomKey(int length = 256)
        {
            using (var r = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[length];
                r.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }
        [TestMethod]
        public async Task TestMethod1()
        {
            var defaultvalue = default(Claim);

            var tokens = new SharedAccessTokenService( null,null, () => Task.FromResult(new KeyPair{ Primary= GetRandomKey(64), Secondary= GetRandomKey(64)}));
            var model = new SasTokenGenerationModel{
                Claims = new List<Claim> { new Claim("exp", "dsadsa"), new Claim("exps", "fdfs"), new Claim("exsp", "sda"), new Claim("exsp", "das") }
            };

            var a = await tokens.GetTokenAsync(model);
            IEnumerable<Claim> claims = await tokens.CheckSignatureAsync(a);
            Assert.AreEqual(true, claims.Any());

            var path = "av";
            var prefix = "a";
            Assert.AreEqual(true, path.StartsWith(prefix));

        
        }
        static Regex _rangeregex = new Regex(@"^bytes=\d*-\d*(,\d*-\d*)*$", RegexOptions.Compiled);
        [TestMethod]
        public async Task TestRangeHeader()
        {
            var value = "bytes=0-";
            var matches = _rangeregex.Match(value);

            foreach (Capture capture in matches.Captures)
            {
                var captureValue = capture.Value;
                if (!string.IsNullOrEmpty(captureValue))
                {
                    var range = captureValue.Substring(6);
                    var rangeParts = range.Split('-').Where(s=>!string.IsNullOrEmpty(s)).ToArray();
                    if (rangeParts.Length == 1)
                    {
                        var a = long.Parse(rangeParts[0]);
                    }
                    else if (rangeParts.Length == 2)
                    {
                      var b = long.Parse(rangeParts[0]);
                      var c = long.Parse(rangeParts[1]);
                    }

                }

            }
        }
        [TestMethod]
        public async Task TestXmlCopy()
        {
            var xml = @"﻿<?xml version=""1.0"" encoding=""utf-8""?><EnumerationResults ServiceEndpoint=""test""><Prefix>0f0f36bcf00244ce8f7096c29076d4cb</Prefix><Containers><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-ascend-system-folder</Name><Properties><Last-Modified>Sun, 01 Mar 2015 15:12:55 GMT</Last-Modified><Etag>""0x8D222495434801B""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-bornholm</Name><Properties><Last-Modified>Tue, 19 May 2015 13:45:30 GMT</Last-Modified><Etag>""0x8D26051347E1E3C""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-bornholm-airporttest</Name><Properties><Last-Modified>Thu, 21 May 2015 19:24:26 GMT</Last-Modified><Etag>""0x8D26212E2C3E366""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-capture-wpoints-gsd6</Name><Properties><Last-Modified>Wed, 13 May 2015 11:30:29 GMT</Last-Modified><Etag>""0x8D25B8759571425""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-default</Name><Properties><Last-Modified>Tue, 10 Mar 2015 15:00:07 GMT</Last-Modified><Etag>""0x8D2295A082B3061""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-deliverytest</Name><Properties><Last-Modified>Sun, 14 Jun 2015 22:58:06 GMT</Last-Modified><Etag>""0x8D2750CB5CA6A58""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-demos</Name><Properties><Last-Modified>Wed, 04 Mar 2015 13:42:45 GMT</Last-Modified><Etag>""0x8D224983AD3F1DA""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-dhm-point-cloud</Name><Properties><Last-Modified>Sat, 28 Feb 2015 01:24:37 GMT</Last-Modified><Etag>""0x8D2210C73A384C8""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-flighttest</Name><Properties><Last-Modified>Tue, 19 May 2015 15:57:48 GMT</Last-Modified><Etag>""0x8D26063B02F3F16""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-garmin</Name><Properties><Last-Modified>Wed, 18 Feb 2015 09:00:01 GMT</Last-Modified><Etag>""0x8D2197069C086B8""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-hello</Name><Properties><Last-Modified>Thu, 02 Apr 2015 18:25:51 GMT</Last-Modified><Etag>""0x8D23B89954CFC31""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-henrik-demo</Name><Properties><Last-Modified>Wed, 13 May 2015 12:31:22 GMT</Last-Modified><Etag>""0x8D25B8FDAE067AE""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-invester</Name><Properties><Last-Modified>Fri, 08 May 2015 08:56:26 GMT</Last-Modified><Etag>""0x8D25784004DD786""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-investermode</Name><Properties><Last-Modified>Fri, 08 May 2015 11:37:21 GMT</Last-Modified><Etag>""0x8D2579A7AF4595A""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-ireland</Name><Properties><Last-Modified>Mon, 11 May 2015 13:41:29 GMT</Last-Modified><Etag>""0x8D25A075201A1BF""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-ireland-2157</Name><Properties><Last-Modified>Wed, 13 May 2015 08:40:52 GMT</Last-Modified><Etag>""0x8D25B6FA780C85A""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-ireland-25</Name><Properties><Last-Modified>Wed, 13 May 2015 19:06:53 GMT</Last-Modified><Etag>""0x8D25BC71BFA3F22""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-ireland-25830</Name><Properties><Last-Modified>Tue, 12 May 2015 08:02:32 GMT</Last-Modified><Etag>""0x8D25AA122793E1F""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-ireland-25830-2</Name><Properties><Last-Modified>Tue, 12 May 2015 13:31:53 GMT</Last-Modified><Etag>""0x8D25ACF24E81F09""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-jesper-luftfotos</Name><Properties><Last-Modified>Thu, 26 Feb 2015 12:28:58 GMT</Last-Modified><Etag>""0x8D21FD6EDE3E332""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-jesper-rebild-bakker</Name><Properties><Last-Modified>Sun, 22 Feb 2015 01:12:39 GMT</Last-Modified><Etag>""0x8D21C53C8D996B8""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-maclachlan-property</Name><Properties><Last-Modified>Mon, 11 May 2015 09:49:33 GMT</Last-Modified><Etag>""0x8D259E6EAEEF744""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-maclachlan-property-5</Name><Properties><Last-Modified>Mon, 01 Jun 2015 09:29:55 GMT</Last-Modified><Etag>""0x8D26A64A786DF43""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-maclachlan-property1</Name><Properties><Last-Modified>Mon, 11 May 2015 10:02:44 GMT</Last-Modified><Etag>""0x8D259E8C27748DE""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-maclachlan-property2</Name><Properties><Last-Modified>Mon, 11 May 2015 10:32:00 GMT</Last-Modified><Etag>""0x8D259ECD963336C""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-maclachlan-property4</Name><Properties><Last-Modified>Mon, 11 May 2015 11:28:40 GMT</Last-Modified><Etag>""0x8D259F4C3D91B74""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-mjk-test</Name><Properties><Last-Modified>Wed, 18 Mar 2015 00:25:34 GMT</Last-Modified><Etag>""0x8D22F292F7FE408""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-mjk-test10</Name><Properties><Last-Modified>Thu, 19 Mar 2015 10:48:59 GMT</Last-Modified><Etag>""0x8D2304970870BC3""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-mjk-test2</Name><Properties><Last-Modified>Thu, 02 Apr 2015 14:25:52 GMT</Last-Modified><Etag>""0x8D23B680EDAC152""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-mjk-test3</Name><Properties><Last-Modified>Mon, 23 Mar 2015 09:24:00 GMT</Last-Modified><Etag>""0x8D233623AFDB02C""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-mjk-test4</Name><Properties><Last-Modified>Wed, 18 Mar 2015 22:13:36 GMT</Last-Modified><Etag>""0x8D22FDFEA46E6CB""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-mjk-test6</Name><Properties><Last-Modified>Mon, 23 Mar 2015 09:23:56 GMT</Last-Modified><Etag>""0x8D2336238D3B77E""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-mjk-test7</Name><Properties><Last-Modified>Mon, 23 Mar 2015 09:23:53 GMT</Last-Modified><Etag>""0x8D2336236DA4624""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-mjk-test8</Name><Properties><Last-Modified>Mon, 23 Mar 2015 09:23:49 GMT</Last-Modified><Etag>""0x8D2336234D40345""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-mjk-test9</Name><Properties><Last-Modified>Mon, 23 Mar 2015 09:23:46 GMT</Last-Modified><Etag>""0x8D23362329018E4""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-mjktest10</Name><Properties><Last-Modified>Sat, 02 May 2015 20:42:47 GMT</Last-Modified><Etag>""0x8D2532FAEC1A599""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-my-cool-test-folder</Name><Properties><Last-Modified>Tue, 02 Jun 2015 09:34:46 GMT</Last-Modified><Etag>""0x8D26B2E7F5FDFE9""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-noblending-nodecimate-tiles</Name><Properties><Last-Modified>Tue, 26 May 2015 18:01:18 GMT</Last-Modified><Etag>""0x8D265F519707047""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-prodtifftest</Name><Properties><Last-Modified>Wed, 03 Jun 2015 11:20:22 GMT</Last-Modified><Etag>""0x8D26C066A6B477A""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-sentionelone</Name><Properties><Last-Modified>Tue, 16 Jun 2015 23:24:19 GMT</Last-Modified><Etag>""0x8D276A2B4788B0F""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-sonderborg-air</Name><Properties><Last-Modified>Sat, 21 Mar 2015 15:06:50 GMT</Last-Modified><Etag>""0x8D231FFCB13D551""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-test</Name><Properties><Last-Modified>Tue, 17 Feb 2015 10:49:31 GMT</Last-Modified><Etag>""0x8D218B68B4F0D59""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-test-work-jesper</Name><Properties><Last-Modified>Fri, 27 Feb 2015 10:26:59 GMT</Last-Modified><Etag>""0x8D2208F0DA1B4D8""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-test0</Name><Properties><Last-Modified>Mon, 20 Apr 2015 13:48:39 GMT</Last-Modified><Etag>""0x8D24987D713A372""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-test2</Name><Properties><Last-Modified>Sun, 01 Mar 2015 15:13:45 GMT</Last-Modified><Etag>""0x8D22249725EA5B9""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-test3</Name><Properties><Last-Modified>Tue, 10 Mar 2015 10:43:33 GMT</Last-Modified><Etag>""0x8D229363077AF73""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-tiffs</Name><Properties><Last-Modified>Thu, 02 Apr 2015 14:41:36 GMT</Last-Modified><Etag>""0x8D23B6A41C3F2A8""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-virocker</Name><Properties><Last-Modified>Sun, 24 May 2015 18:59:31 GMT</Last-Modified><Etag>""0x8D2646AE70CA45D""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container><Container><Name>0f0f36bcf00244ce8f7096c29076d4cb-virocker-tiles</Name><Properties><Last-Modified>Sun, 24 May 2015 21:05:01 GMT</Last-Modified><Etag>""0x8D2647C6F5CBDC0""</Etag><LeaseStatus>unlocked</LeaseStatus><LeaseState>available</LeaseState></Properties></Container></Containers><NextMarker/></EnumerationResults>";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine(xml);
            writer.Flush();
            stream.Position=0;


         
                var ms = new MemoryStream();
                await DefaultRequestHandlerService.WriteXmlAsync(stream, ms, (s) => s.Substring("0f0f36bcf00244ce8f7096c29076d4cb".Length + 1), new ListOptions(), "Container");

                ms.Position = 0;
                Console.WriteLine(new StreamReader(ms).ReadToEnd());
        
        }
    }
}
