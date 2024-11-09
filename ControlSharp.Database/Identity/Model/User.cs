using Microsoft.AspNetCore.Identity;

namespace ControlSharp.Database.Identity.Model;

public class User : IdentityUser<Guid>
{
    public DateTimeOffset? Created { get; set; }
    public DateTimeOffset? LastOnline { get; set; }
}