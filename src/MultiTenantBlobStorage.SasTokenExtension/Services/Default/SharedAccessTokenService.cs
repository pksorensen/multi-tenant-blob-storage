using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SInnovations.Azure.MultiTenantBlobStorage.Extensions;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.Default
{
    public class KeyPair
    {
        public string Primary { get; set; }
        public string Secondary { get; set; }
    }
    public class SharedAccessTokenService : ISharedAccessTokenService
    {

        private readonly Lazy<Task<KeyPair>> _keyProvider;

        public SharedAccessTokenService(Func<Task<KeyPair>> keysProvider)
        {
            _keyProvider = new Lazy<Task<KeyPair>>(keysProvider);
        }
        public virtual IEnumerable<Claim> GetClaimsForToken(IOwinContext context, ResourceContext resourceContext )
        {
            var expireTIme = DateTimeOffset.UtcNow.AddHours(4);
            String dateInRfc1123Format = expireTIme.ToString("R", CultureInfo.InvariantCulture);

            //Expire
          
                yield return new Claim("exp", dateInRfc1123Format);
            //notbefore 
           
                yield return new Claim("nbf", DateTimeOffset.UtcNow.ToString("R", CultureInfo.InvariantCulture));
            
            yield return new Claim("role", "read");
            yield return new Claim("role", "write");
            yield return new Claim("tenant", resourceContext.Route.TenantId);
            yield return new Claim("resource", resourceContext.Route.Resource);
            if (!string.IsNullOrWhiteSpace(resourceContext.Route.Path))
            {
                yield return new Claim("prefix", resourceContext.Route.Path);
            
            }

        }
        public virtual async Task<IEnumerable<Claim>> CheckSignatureAsync(string token)
        {
           
            var parts = token.Split('.');
            if (parts.Length != 3)
                return Enumerable.Empty<Claim>();

            string incoming = parts[1]
                .Replace('_', '/').Replace('-', '+');
            switch (parts[1].Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }
            byte[] bytes = Convert.FromBase64String(incoming);
            string originalText = Encoding.UTF8.GetString(bytes);
            var claims = (JObject.Parse(originalText).Properties().Select(p => new Claim(p.Name, p.Value.ToString()))).ToArray();

            var keys = await _keyProvider.Value;

            if ((string.Equals(parts[2], CalculateSignature(token, keys.Primary), StringComparison.OrdinalIgnoreCase) 
                || string.Equals(parts[2], CalculateSignature(token,keys.Secondary),StringComparison.OrdinalIgnoreCase)))
                return claims;

            return Enumerable.Empty<Claim>();
            //
            
            
            //var claims = 

            //var signature = parts[2];
            //JToken expToken, nbfToken,roleToken;
            //if (!(claims.TryGetValue("exp", out expToken) && expToken.Type == JTokenType.String))
            //    return false;
            //if (!(claims.TryGetValue("nbf", out nbfToken) && nbfToken.Type == JTokenType.String))
            //    return false;
            //if (!(claims.TryGetValue("role", out roleToken) && roleToken.Type == JTokenType.Array))
            //    return false;


            //var exp = DateTimeOffset.Parse(expToken.ToString(), CultureInfo.InvariantCulture);
            //String expInRfc1123Format = exp.ToString("R", CultureInfo.InvariantCulture);
            //var nbf = DateTimeOffset.Parse(nbfToken.ToString(), CultureInfo.InvariantCulture);
            //String nbfInRfc1123Format = nbf.ToString("R", CultureInfo.InvariantCulture);
            //var role = roleToken.ToObject<string[]>();

          
            
          



            //return true;
        }

        private string CalculateSignature(string token, string key)
        {
            using (HMACSHA256 hmacSha256 = new HMACSHA256(Convert.FromBase64String(key)))
            {

                return Convert.ToBase64String(hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(token.Substring(0,token.LastIndexOf('.')))));

            }
        }
        public virtual async Task<string> GetTokenAsync(IEnumerable<Claim> claims)
        {
            var body= string.Format("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.{0}", 
                Base64UrlEncode(string.Format("{{{0}}}",string.Join(",",claims.GroupBy(o=>o.Type).OrderBy(o=>o.Key).Select(GetStringProperty)))));

            var keys = await _keyProvider.Value;

            using (HMACSHA256 hmacSha256 = new HMACSHA256(Convert.FromBase64String(keys.Primary)))
            {

                var signature = Base64UrlEncode(hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(body)));
                return string.Format("{0}.{1}", body, signature);
            }

        
        }

        private string GetStringProperty(IGrouping<string, Claim> arg)
        {
            return string.Format("\"{0}\":{1}", arg.Key, GetValue(arg));
        }

        private string GetValue(IGrouping<string, Claim> arg)
        {
            if (arg.Skip(1).Any())
                return string.Format("[{0}]", string.Join(",", arg.Select(a=>string.Format("\"{0}\"", a.Value))));
            return string.Format("\"{0}\"", arg.First().Value);
        }
        static readonly char[] padding = { '=' };
        public string Base64UrlEncode(string input)
        {
            return Base64UrlEncode(Encoding.UTF8.GetBytes(input));

        }
        public string Base64UrlEncode(byte[] input)
        {
            return System.Convert.ToBase64String(input)
          .TrimEnd(padding).Replace('+', '-').Replace('/', '_');
        }
       
    }
}
