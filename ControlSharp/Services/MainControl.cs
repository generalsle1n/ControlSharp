using ControlSharp.Config;
using Microsoft.EntityFrameworkCore;

namespace ControlSharp.Services;

public class MainControl : IHostedService
{
    private readonly IServiceScope _scope;
    private readonly DatabaseContext _context;
    public MainControl(IServiceScopeFactory ServiceScopeFactory)
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
    }
}