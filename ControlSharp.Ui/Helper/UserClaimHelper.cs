using System.Security.Claims;
using ControlSharp.Database.Identity.Model;
using Microsoft.AspNetCore.Identity;

namespace ControlSharp.Ui.Helper;

public class UserClaimHelper
{
    internal const string ApiClaimId = "ApiTokenIdentity";

    public static async Task<string> GetClaimValueFromUser(string userMail, string claimName, SignInManager<User> signInManager)
    {
        User user = await signInManager.UserManager.FindByEmailAsync(userMail);
        IList<Claim> AllClaims = await signInManager.UserManager.GetClaimsAsync(user);
        
        Claim Result = AllClaims.Where(claim => claim.Type.Equals(claimName)).First();
        
        return Result.Value;
    }
}