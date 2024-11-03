using Microsoft.AspNetCore.Identity;

namespace ControlSharp.Api.Config.Model;

public class User : IdentityUser
{
    public DateTimeOffset? Created { get; set; }
    public DateTimeOffset? LastOnline { get; set; }
}