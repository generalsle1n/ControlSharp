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
    private readonly ILogger<MainControl> _logger;
    private const string _adminName = "admin@local";
    
    public MainControl(IServiceScopeFactory ServiceScopeFactory, ILogger<MainControl> Logger)
    {
        _scope = ServiceScopeFactory.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
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
                UserName = _adminName,
                Email = _adminName,
                Created = DateTime.Now
            };

            string InitalPassword = SecretManager.CreateAdminToken();
            await _userManager.CreateAsync(InitialAdmin, InitalPassword);
            
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