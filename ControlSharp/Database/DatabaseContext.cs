using ControlSharp.Api.Config.Model;
using Microsoft.EntityFrameworkCore;

namespace ControlSharp.Api.Config;

public class DatabaseContext : DbContext
{
    public DbSet<Session> Session { get; set; }
    public DbSet<Asset> Asset { get; set; }
    public DbSet<User> User { get; set; }
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }
}