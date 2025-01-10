using Duende.IdentityServer.Models;
using IdentityModel;

namespace ControlSharp.Identity.Config.IdentityResourcesConfig
{
    public class AllIdentityResources
    {
        internal static List<IdentityResource> Resources = new List<IdentityResource>()
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Address(),
            new IdentityResource()
            {
                Name = "roles",
                DisplayName = "Object Roles",
                UserClaims = new List<string>
                {
                    JwtClaimTypes.Role
                }
            }
        };
    }
}
