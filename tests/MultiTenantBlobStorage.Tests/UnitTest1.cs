﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SInnovations.Azure.MultiTenantBlobStorage.Services.Default;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

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

            var tokens = new SharedAccessTokenService( () => Task.FromResult(new KeyPair{ Primary= GetRandomKey(64), Secondary= GetRandomKey(64)}));

            var a = await tokens.GetTokenAsync(new List<Claim> { new Claim("exp", "dsadsa"), new Claim("exps", "fdfs"), new Claim("exsp", "sda"), new Claim("exsp", "das") });
            IEnumerable<Claim> claims = await tokens.CheckSignatureAsync(a);
            Assert.AreEqual(true, claims.Any());

            var path = "av";
            var prefix = "a";
            Assert.AreEqual(true, path.StartsWith(prefix));

        
        }
    }
}
