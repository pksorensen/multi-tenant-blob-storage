using SInnovations.Azure.MultiTenantBlobStorage.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.Default
{
    public class DefaultResourceAuthorizationManager : ResourceAuthorizationManager
    {
        static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public override Task<bool> CheckAccessAsync(ResourceAuthorizationContext context)
        {

            //TRACE EVERYTHING. We will have to clean out logs every week, month or something.
            Logger.Trace("\n\nClaimsAuthorizationManager\n_______________________\n");

            Logger.Trace("\nAction:");
            Logger.Trace("  " + context.Action.First().Value);

            Logger.Trace("\nResources:");
            foreach (var resource in context.Resource)
            {
                Logger.Trace("  " + resource.Value);
            }

            Logger.Trace("\nClaims:");
            foreach (var claim in context.Principal.Claims)
            {
                Logger.Trace("  " + claim.Value);
            }
            Logger.Trace("  " + context.Action.First().Value);

            //Handle Authorization. So it dont need to be in a big switch, we need to figure out how we want to this.
            //I just want authoriation seperated from business logic, and unit tests can be made on the authorizationManager.

            var action = context.Action.First().Value;






            return Ok();
        }


    }
}
