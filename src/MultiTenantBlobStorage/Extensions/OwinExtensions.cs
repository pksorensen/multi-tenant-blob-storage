using Microsoft.Owin;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Extensions
{
    public static class OwinExtensions
    {
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
    }
}
