using ControlSharp.Config;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;


namespace ControlSharp.Services;

public class MainControl : IHostedService
{
    private readonly IServiceScope _scope;
    private readonly DatabaseContext _context;
    public MainControl(IServiceScopeFactory ServiceScopeFactory)
    private const int _adminKeySize = 128;
    {
        _scope = ServiceScopeFactory.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<DatabaseContext>();
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
        _context.Database.EnsureCreated();
    private string CreateAdminToken()
    {
        byte[] TokenData = RandomNumberGenerator.GetBytes(_adminKeySize);
        string Secret = Convert.ToBase64String(TokenData);
        
        return Secret;
    }
}