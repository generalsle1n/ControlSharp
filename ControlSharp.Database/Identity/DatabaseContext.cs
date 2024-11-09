using ControlSharp.Database.Identity.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ControlSharp.Database.Identity;

public class DatabaseContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<Asset> Asset { get; set; }
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }
}