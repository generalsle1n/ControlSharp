using ControlSharp.Config.Model;
using Microsoft.EntityFrameworkCore;

namespace ControlSharp.Config;

public class DatabaseContext : DbContext
{
    DbSet<Access> Accesses { get; set; }
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }
}