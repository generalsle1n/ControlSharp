using ControlSharp.Api.Database.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ControlSharp.Api.Database;

public class DatabaseContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<Asset> Asset { get; set; }
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }
}