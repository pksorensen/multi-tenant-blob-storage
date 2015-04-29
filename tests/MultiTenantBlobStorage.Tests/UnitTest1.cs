using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SInnovations.Azure.MultiTenantBlobStorage.Services.Default;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using SInnovations.Azure.MultiTenantBlobStorage.SasTokenExtension.Models;

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
    }
}
