using Microsoft.EntityFrameworkCore;
using ControlSharp.Model.Database.Assets;
using System.Runtime.CompilerServices;

namespace ControlSharp.Api.Database
{
    public class DatabaseContext : DbContext
    {
        internal DbSet<Asset> Assets { get; set; }

        private ILogger<DatabaseContext> _logger;

        public DatabaseContext(DbContextOptions<DatabaseContext> options, ILogger<DatabaseContext> Logger) : base(options)
        {
            _logger = Logger;
        }
    }
}
