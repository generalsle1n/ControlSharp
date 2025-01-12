using Duende.IdentityServer.Models;

namespace ControlSharp.Identity.Config.ApiResourceConfig
{
    public class ControlSharpApiResource
    {
        internal static ApiResource Resource = new ApiResource()
        {
            UserClaims = new List<string>
            {
                "role"
            },
            Scopes = new List<string>()
            {
                "ControlSharp-Api"
            }
        };
    }
}
