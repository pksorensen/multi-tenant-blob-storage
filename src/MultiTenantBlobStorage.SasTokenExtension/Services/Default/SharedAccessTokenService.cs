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
    public class SharedAccessTokenService : ISharedAccessTokenService
    {
        private readonly string primary;
        private readonly string secondary;

        public SharedAccessTokenService(string primary, string secondary)
        {
            this.primary = primary;
            this.secondary = secondary;
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
        public virtual bool CheckSignature(string token, out IEnumerable<Claim> claims)
        {
            claims = null;
            var parts = token.Split('.');
            if (parts.Length != 3)
                return false;

            string incoming = parts[1]
                .Replace('_', '/').Replace('-', '+');
            switch (parts[1].Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }
            byte[] bytes = Convert.FromBase64String(incoming);
            string originalText = Encoding.UTF8.GetString(bytes);
            claims = JObject.Parse(originalText).Properties().Select(p => new Claim(p.Name, p.Value.ToString())).ToArray();

            if ((string.Equals(parts[2], CalculateSignature(token,primary),StringComparison.OrdinalIgnoreCase) 
                || string.Equals(parts[2], CalculateSignature(token,secondary),StringComparison.OrdinalIgnoreCase)))
                return true;

            return false;
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
        public virtual string GetToken(IEnumerable<Claim> claims)
        {
            var body= string.Format("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.{0}", 
                Base64UrlEncode(string.Format("{{{0}}}",string.Join(",",claims.OrderBy(o=>o.Type).Select(c=>string.Format("\"{0}\":\"{1}\"",c.Type,c.Value))))));

            using (HMACSHA256 hmacSha256 = new HMACSHA256(Convert.FromBase64String(primary)))
            {

                var signature = Convert.ToBase64String(hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(body)));

                return string.Format("{0}.{1}", body, signature);
            }
        
        }
        static readonly char[] padding = { '=' };
        public string Base64UrlEncode(string input)
        {
            return System.Convert.ToBase64String(Encoding.UTF8.GetBytes(input))
          .TrimEnd(padding).Replace('+', '-').Replace('/', '_'); 
        }

       
    }
}
