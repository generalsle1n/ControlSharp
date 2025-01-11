using Duende.IdentityServer.Models;

namespace ControlSharp.Identity.Config.Clients
{
    internal class ClientGenerator
    {
        internal static List<Client> CreateClients(IConfiguration configuration)
        {
            List<Client> Result = new List<Client>();
            Result.Add(CreateUiClient(configuration));
            Result.Add(CreateApiClient(configuration));
            return Result;
        }

        private static Client CreateUiClient(IConfiguration configuration)
        {
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

            return ControlSharpUi;
        }

        private static Client CreateApiClient(IConfiguration configuration)
        {
            Client ControlSharpApi = ControlSharpApiClient.Client;
            
            ControlSharpApi.AllowedScopes.Add(configuration.GetValue<string>("ControlSharpApiOICDId"));
            ControlSharpApi.ClientId = configuration.GetValue<string>("ControlSharpApiOICDId");

            return ControlSharpApi;
        }
    }
}
