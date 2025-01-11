using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace ControlSharp.Identity.Config.Clients
{
    internal class ControlSharpUiClient
    {
        private const string _name = "ControlSharp-Ui";
        internal static Client Client = new Client()
        {
            ClientName = _name,
            AllowedGrantTypes = GrantTypes.Code,
            RequirePkce = true,
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "roles",
                "ControlSharp-Api"
            }
        };
    }
}
