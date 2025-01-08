using Duende.IdentityServer.Models;

namespace ControlSharp.Identity.Config.IdentityResourcesConfig
{
    public class AllIdentityResources
    {
        internal static List<IdentityResource> Resources = new List<IdentityResource>()
        {
            new IdentityResources.OpenId()
        };
    }
}
