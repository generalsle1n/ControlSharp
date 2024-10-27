using ControlSharp.Config.Model;
using Microsoft.EntityFrameworkCore;

namespace ControlSharp.Config;

public class DatabaseContext : DbContext
{
    public DbSet<Access> Access { get; set; }
    public DbSet<Asset> Asset { get; set; }
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }
}