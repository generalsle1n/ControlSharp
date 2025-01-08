using Duende.IdentityServer.Models;

namespace ControlSharp.Identity.Config.Clients
{
    internal class ClientGenerator
    {
        internal static List<Client> CreateClients(IConfiguration configuration)
        {
            List<Client> Result = new List<Client>();
            Client ControlSharpUi = ControlSharpUiClient.Client;
            ControlSharpUi.ClientId = configuration.GetValue<string>("ControlSharpUiOICDId");
            ControlSharpUi.ClientSecrets = new List<Secret>()
            {
                new Secret(configuration.GetValue<string>("ControlSharpUiOICDSecret").Sha256())
            };

            ControlSharpUi.RedirectUris = new List<string>()
            {
                $"{configuration.GetValue<string>("ControlSharpUiServer")}/signin-oidc"
            };
            ControlSharpUi.PostLogoutRedirectUris = new List<string>()
            {
                $"{configuration.GetValue<string>("ControlSharpUiServer")}/signout-callback-oidc"
            };

            Result.Add(ControlSharpUi);
            return Result;
        }
    }
}
