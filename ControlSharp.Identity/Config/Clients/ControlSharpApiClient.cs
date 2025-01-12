using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace ControlSharp.Identity.Config.Clients
{
    internal class ControlSharpApiClient
    {
        private const string _name = "ControlSharp-Api";
        internal static Client Client = new Client()
        {
            ClientName = _name,
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            RequirePkce = true,
            AccessTokenType = AccessTokenType.Reference,
            AllowedScopes = new List<string>()
            {
                
            }
        };
    }
}
