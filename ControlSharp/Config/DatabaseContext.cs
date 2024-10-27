using Microsoft.EntityFrameworkCore;

namespace ControlSharp.Config;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
    {
        
    }
}