using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SInnovations.Azure.MultiTenantBlobStorage.Services.Default;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Security.Claims;

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
        public void TestMethod1()
        {
            var tokens = new SharedAccessTokenService(GetRandomKey(64), GetRandomKey(64));

            var a = tokens.GetToken(new List<Claim> { new Claim("exp", "dsadsa"), new Claim("exps", "fdfs"), new Claim("exsp", "sda"), new Claim("exsp", "das") });
            IEnumerable<Claim> claims;
            Assert.AreEqual(true, tokens.CheckSignature(a,out claims));

            var path = "av";
            var prefix = "a";
            Assert.AreEqual(true, path.StartsWith(prefix));

        
        }
    }
}
