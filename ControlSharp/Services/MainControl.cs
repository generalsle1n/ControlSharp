using ControlSharp.Api.Config;
using ControlSharp.Api.Config.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;


namespace ControlSharp.Services;

public class MainControl : IHostedService
{
    private readonly IServiceScope _scope;
    private readonly DatabaseContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ILogger<MainControl> _logger;
    private const string _adminName = "admin";
    private const string _adminDomain = "local";
    private const string _adminEmail = $"{_adminName}@{_adminDomain}";
    
    public MainControl(IServiceScopeFactory ServiceScopeFactory, ILogger<MainControl> Logger)
    {
        _scope = ServiceScopeFactory.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        _roleManager = _scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        _logger = Logger;
    }
    
    public async Task StartAsync(CancellationToken Token)
    {
        await StartUp(Token);       
    }

    public Task StopAsync(CancellationToken Token)
    {
        return Task.CompletedTask;
    }

    private async Task StartUp(CancellationToken Token)
    {
        bool Created = _context.Database.EnsureCreated();
        
        if (Created)
        {
            _logger.LogInformation("Created Database");
            
            User InitialAdmin = new User()
            {
                UserName = _adminEmail,
                Email = _adminEmail,
                Created = DateTime.Now
            };
            
            string InitalPassword = SecretManager.CreateAdminToken();
            await _userManager.CreateAsync(InitialAdmin, InitalPassword);
            
            Role AdminRole = new Role()
            {
                Name = AccessRole.Super.ToString()
            };
            
            await _roleManager.CreateAsync(AdminRole);
            
            await _userManager.AddToRoleAsync(InitialAdmin, AccessRole.Super.ToString());
            
            _logger.LogInformation("Created Admin");
            _logger.LogInformation($"UserID: {InitialAdmin.Email}");
            _logger.LogInformation($"Key: {InitalPassword}");
            
        }
        else
        {
            _logger.LogInformation("Database found");
        }
    }

    
}