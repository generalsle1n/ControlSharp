using ControlSharp.Database.Identity.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ControlSharp.Database.Identity;

public class DatabaseContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<Asset> Asset { get; set; }
    private ILogger<DatabaseContext> _logger;
    private const string _adminName = "admin";
    private const string _adminDomain = "local";
    private const string _adminEmail = $"{_adminName}@{_adminDomain}";
    public DatabaseContext(DbContextOptions<DatabaseContext> options, ILogger<DatabaseContext> Logger) : base(options)
    {
        _logger = Logger;
    }

    public async Task CheckDatabaseAsync(UserManager<User> userManager, RoleManager<Role> roleManager, bool CreateDatabase)
    {
        if (CreateDatabase)
        {
            bool Created = await Database.EnsureCreatedAsync();

            if (Created)
            {
                _logger.LogInformation($"Created Database: {Database.GetConnectionString()}");
                User User = await CreateUserAsync(userManager);
                Role Role = await CreateRoleAsync(roleManager);
                await AddUserToRoleAsync(userManager, roleManager, User, Role);
            }
            else
            {
                _logger.LogInformation($"Database already exists: {Database.GetConnectionString()}");
            }   
        }
    }

    private async Task<User> CreateUserAsync(UserManager<User> userManager)
    {
        User InitialAdmin = new User()
        {
            UserName = _adminEmail,
            Email = _adminEmail,
            Created = DateTime.Now
        };
        
        string InitalPassword = SecretManager.CreateAdminToken();
        await userManager.CreateAsync(InitialAdmin, InitalPassword);
        _logger.LogInformation($"Created User: {InitialAdmin.UserName}");
        _logger.LogInformation($"User password: {InitalPassword}");
        
        return InitialAdmin;
    }
    private async Task<Role> CreateRoleAsync(RoleManager<Role> roleManager)
    {
        Role SuperRole = new Role()
        {
            Name = nameof(AccessRole.Super)
        };

        Role AssetRole = new Role()
        {
            Name = nameof(AccessRole.Asset)
        };

        Role ReadOnlyRole = new Role()
        {
            Name = nameof(AccessRole.ReadOnly)
        };

        Role AdminRole = new Role()
        {
            Name = nameof(AccessRole.Admin)
        };
        
        await roleManager.CreateAsync(SuperRole);
        _logger.LogInformation($"Created Role: {SuperRole.Name}");

        await roleManager.CreateAsync(AssetRole);
        _logger.LogInformation($"Created Role: {AssetRole.Name}");

        await roleManager.CreateAsync(ReadOnlyRole);
        _logger.LogInformation($"Created Role: {ReadOnlyRole.Name}");

        await roleManager.CreateAsync(AdminRole);
        _logger.LogInformation($"Created Role: {AdminRole.Name}");

        return SuperRole;
    }
    private async Task AddUserToRoleAsync(UserManager<User> userManager, RoleManager<Role> roleManager, User User, Role Role)
    {
        List<string> RoleList = new List<string>();
        RoleList.Add(Role.Name);
        
        await userManager.AddToRolesAsync(User, RoleList);
        _logger.LogInformation($"Added Role: {Role.Name} to User: {User.UserName}");
    }
}