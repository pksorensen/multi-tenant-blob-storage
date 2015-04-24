using Microsoft.Owin;
using Microsoft.Practices.Unity;
using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace SInnovations.Azure.MultiTenantBlobStorage.Extensions
{
    public static class OwinExtensions
    {
        static Regex _rangeregex = new Regex(@"^bytes=\d*-\d*(,\d*-\d*)*$",RegexOptions.Compiled);
        static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
        public static IList<string> UserAgent(this IHeaderDictionary headers)
        {
            return headers.GetValues(Constants.HeaderConstants.UserAgent);
        }
        public static void CopyTo(this WebHeaderCollection headers, string header, IOwinResponse response, params string[] defaultValues)
        {
            var values = headers.GetValues(header) ?? defaultValues;
            if (values == null || !values.Any())
                return;
            switch (header.ToLower())
            {
                case Constants.HeaderConstants.ContentType:
                    response.ContentType = string.Join(", ", values);
                    return;
                case Constants.HeaderConstants.ContentLenght:
                    response.ContentLength = long.Parse(values.First());
                    return;
                case Constants.HeaderConstants.TansferEncoding:
                    var action = response.Context.Get<Action>("server.DisableResponseBuffering");
                    if (action != null)
                    {
                        action();
                    }
                    return;
               
            }


            response.Headers.SetValues(header,values);
            
        }
        public static void CopyTo(this IHeaderDictionary headers, string header, HttpWebRequest request, params string[] defaultValues)
        {
 
            var values = headers.GetValues(header) ?? defaultValues;
            if(values == null ||!values.Any())
                return;

            switch (header.ToLower())
            {
                case Constants.HeaderConstants.UserAgent :
                    request.UserAgent = string.Join(", ", values);
                    return;
                case Constants.HeaderConstants.TansferEncoding:
                    request.TransferEncoding = string.Join(", ", values);
                    return;
                case Constants.HeaderConstants.Referer:
                    request.Referer = string.Join(", ", values);
                    return;
                case Constants.HeaderConstants.Range:
                   
                    foreach(var value in values){
                        var matches = _rangeregex.Match(value);
                        
                        foreach(Capture capture in matches.Captures)
                        {
                            var captureValue = capture.Value;
                            if(captureValue.IsPresent()){
                                var range = captureValue.Substring(6);
                                var rangeParts = range.Split('-');
                                if(rangeParts.Length==1)
                                {
                                    request.AddRange(long.Parse(rangeParts[0]));
                                }else if(rangeParts.Length ==2)
                                {
                                    request.AddRange(long.Parse(rangeParts[0]), long.Parse(rangeParts[1]));
                                }
                               
                            }
                            
                        }

                       //request.AddRange(int.Parse(value));
                   }
                   
                    return;
                case Constants.HeaderConstants.ProxyConnection:
                    request.Proxy = new WebProxy(values.First());
                    return;
                case Constants.HeaderConstants.IfModifiedSince:
                    request.IfModifiedSince = DateTime.Parse(values.First());
                    return;
                case Constants.HeaderConstants.Host:
                    request.Host = string.Join(", ", values);
                    return;
                case Constants.HeaderConstants.Expect:
                    request.Expect = string.Join(", ", values);
                    return;
                case Constants.HeaderConstants.Date:
                    request.Date = DateTime.Parse(values.First());
                    return;
                case Constants.HeaderConstants.ContentType:
                    request.ContentType = string.Join(", ", values);
                    return;
                case Constants.HeaderConstants.ContentLenght:
                    request.ContentLength = long.Parse(values.First());
                    return;
                case Constants.HeaderConstants.Connection:
                    request.Connection = string.Join(", ", values);
                    return;
                case Constants.HeaderConstants.Accept:
                    request.Accept = string.Join(", ", values);
                    return;

                default:
                    break;
            }
//Accept
//Connection
//Content-Length
//Content-Type
//Date
//Expect
//Host
//If-Modified-Since
//Range
//Referer
//Transfer-Encoding
//User-Agent
//Proxy-Connection

            try { 

            foreach(var value in values)
                request.Headers.Add(header,value);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Could not copy {0} {1}", ex,header,string.Join(",",values));
            }
        }
        /// <summary>
        /// Gets the current request identifier.
        /// </summary>
        /// <param name="env">The OWIN environment.</param>
        /// <returns></returns>
        public static string GetRequestId(this IDictionary<string, object> env)
        {
            object value;
            if (env.TryGetValue(Constants.OwinContextProperties.RequestId, out value))
            {
                return value as string;
            }

            return null;
        }
        internal static string GetRequestId(this IOwinContext ctx)
        {
            return ctx.Environment.GetRequestId();
        }

        internal static void SetRequestId(this IDictionary<string, object> env, string id)
        {
            env[Constants.OwinContextProperties.RequestId] = id;
        }

        public static T ResolveDependency<T>(this IDictionary<string, object> env)
        {
            var ctx = new OwinContext(env);
            return ctx.ResolveDependency<T>();
            //  var scope = ctx.Get<IUnityContainer>(Constants.OwinContextProperties.UnityRequestContainer);
            //  var instance = scope.Resolve<T>();
            //  return instance;
        }

        internal static T ResolveDependency<T>(this IOwinContext ctx)
        {
            var scope = ctx.Get<IUnityContainer>(Constants.OwinContextProperties.UnityRequestContainer);
            return scope.Resolve<T>();

        }
        

        public static async Task<bool> CheckAccessAsync(this IOwinContext context, ResourceAuthorizationContext authorizationContext)
        {
            return await context.GetAuthorizationManager().CheckAccessAsync(authorizationContext);
        }

        private static IResourceAuthorizationManager GetAuthorizationManager(this IOwinContext context)
        {
            var am = context.Get<IResourceAuthorizationManager>(ResourceAuthorizationManagerMiddleware.Key);

            if (am == null)
            {
                throw new InvalidOperationException("No AuthorizationManager set.");
            }

            return am;
        }
    }
}
