using Duende.IdentityServer.Models;

namespace ControlSharp.Identity.Config.Scopes
{
    internal class AllScopes
    {
        internal static List<ApiScope> AppScopes = new List<ApiScope>()
        {
            new ApiScope(name: "ControlSharp-Api")
        };
    }
}
