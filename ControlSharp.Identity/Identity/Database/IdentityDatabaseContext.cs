using ControlSharp.Identity.Identity.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ControlSharp.Identity.Identity.Database
{
    public class IdentityDatabaseContext : IdentityDbContext<UserIdentity>
    {
        public IdentityDatabaseContext(DbContextOptions<IdentityDatabaseContext> options) : base(options)
        {

        }
    }
}
