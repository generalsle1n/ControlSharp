using ControlSharp.Api.Config;
using ControlSharp.Api.Config.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;


namespace ControlSharp.Services;

public class MainControl : IHostedService
{
    private readonly IServiceScope _scope;
    private readonly DatabaseContext _context;
    private readonly ILogger<MainControl> _logger;
    private const int _adminKeySize = 128;
    private const string _adminName = "admin";
    public MainControl(IServiceScopeFactory ServiceScopeFactory, ILogger<MainControl> Logger)
    {
        _scope = ServiceScopeFactory.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<DatabaseContext>();
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
                Password = CreateAdminToken(),
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

    private string CreateAdminToken()
    {
        byte[] TokenData = RandomNumberGenerator.GetBytes(_adminKeySize);
        string Secret = Convert.ToBase64String(TokenData);
        
        return Secret;
    }
}