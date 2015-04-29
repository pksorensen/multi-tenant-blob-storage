using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.Default
{
    public class DefaultRequestTenantResolver : IRequestTenantResolver
    {
        protected  ITenantContainerNameService  ContainerNameService{get;set;}
        protected IStorageAccountResolverService StorageAccountResolverService{get;set;}

        public DefaultRequestTenantResolver(ITenantContainerNameService s1, IStorageAccountResolverService s2)
        {
            ContainerNameService = s1;
            StorageAccountResolverService = s2;
        }
        public async Task<TenantRoute> GetRouteAsync(Microsoft.Owin.IOwinRequest owinRequest)
        {
            var parts = owinRequest.Path.Value.Trim('/').Split('/');

            var route = new TenantRoute();
           
            if(parts.Any())
                route.TenantId = parts[0];
            if (parts.Length > 1)
                route.Resource = parts[1];

            route.ContainerName = await ContainerNameService.GetContainerNameAsync(route.TenantId,route.Resource);
            route.Host = await StorageAccountResolverService.GetBlobEndpointAsync(route);

            var resourceId = string.Format("{0}/{1}",route.TenantId,  route.Resource);
            var idx = owinRequest.Uri.AbsoluteUri.IndexOf(resourceId) + resourceId.Length;
            var pathAndQuery = owinRequest.Uri.AbsoluteUri.Substring(idx);
            var qIdx = pathAndQuery.IndexOf('?');

            route.Path = Uri.UnescapeDataString( (qIdx > -1 ? pathAndQuery.Substring(0, qIdx) : pathAndQuery).TrimStart('/')) ;

            return route;
        }
    }
}
