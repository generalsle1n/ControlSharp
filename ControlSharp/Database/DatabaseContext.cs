using ControlSharp.Api.Config.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ControlSharp.Api.Config;

public class DatabaseContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<Asset> Asset { get; set; }
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }
}