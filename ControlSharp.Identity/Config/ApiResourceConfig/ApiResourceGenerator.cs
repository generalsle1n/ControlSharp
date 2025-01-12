using ControlSharp.Identity.Config.Clients;
using Duende.IdentityServer.Models;

namespace ControlSharp.Identity.Config.ApiResourceConfig
{
    internal class ApiResourceGenerator
    {
        internal static List<ApiResource> CreateClients(IConfiguration configuration)
        {
            List<ApiResource> Result = new List<ApiResource>();
            Result.Add(CreateApiResource(configuration));
            return Result;
        }

        private static ApiResource CreateApiResource(IConfiguration configuration)
        {
            ApiResource ControlSharpApi = ControlSharpApiResource.Resource;

            ControlSharpApi.Name = configuration.GetValue<string>("ControlSharpApiOICDId");

            ControlSharpApi.ApiSecrets = new List<Secret>()
            {
                new Secret(configuration.GetValue<string>("ControlSharpApiOICDSecret").Sha256())
            };

            return ControlSharpApi;
        }

    }
}
