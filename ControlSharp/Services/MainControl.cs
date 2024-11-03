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
    
    public Task StartAsync(CancellationToken Token)
    {
        StartUp(Token);       
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken Token)
    {
        return Task.CompletedTask;
    }

    private void StartUp(CancellationToken Token)
    {
        bool Created = _context.Database.EnsureCreated();
        
        if (Created)
        {
            _logger.LogInformation("Created Database");
            User Admin = new User()
            {
                Id = Guid.NewGuid(),
                Created = DateTimeOffset.Now,
                UserName = _adminName,
                Password = SecretManager.CreateAdminToken(),
                Active = true,
                Role = AccessRole.Admin
            };
            
            _context.User.Add(Admin);
            _context.SaveChanges();
            
            _logger.LogInformation("Created Admin");
            _logger.LogInformation($"UserID: {Admin.UserName}");
            _logger.LogInformation($"Key: {Admin.Password}");
            
        }
        else
        {
            _logger.LogInformation("Database found");
        }
    }

    
}