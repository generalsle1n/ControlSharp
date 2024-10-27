using ControlSharp.Config;
using ControlSharp.Config.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;


namespace ControlSharp.Services;

public class MainControl : IHostedService
{
    private readonly IServiceScope _scope;
    private readonly DatabaseContext _context;
    private readonly ILogger<MainControl> _logger;
    private const int _adminKeySize = 128;
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
            Access Admin = new Access()
            {
                Id = Guid.NewGuid(),
                Asset = new Asset()
                {
                    Id = Guid.Empty,
                    Created = DateTimeOffset.Now,
                    Name = "Admin"
                },
                Key = new ApiKey()
                {
                    Id = Guid.NewGuid(),
                    Active = true,
                    Created = DateTimeOffset.Now,
                    Key = CreateAdminToken()
                }
            };
            
            _context.Access.Add(Admin);
            _context.SaveChanges();
            
            _logger.LogInformation("Created Admin");
            _logger.LogInformation($"UserID: {Admin.Asset.Id}");
            _logger.LogInformation($"Key: {Admin.Key.Key}");
            
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