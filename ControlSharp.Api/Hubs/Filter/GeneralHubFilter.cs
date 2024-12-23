using ControlSharp.Api.Controller.AssetController;
using ControlSharp.Database.Identity;
using Microsoft.AspNetCore.SignalR;

namespace ControlSharp.Api.Hubs.Filter;

public class GeneralHubFilter : IHubFilter
{
    private readonly DatabaseContext _context;
    private readonly ILogger<GeneralHubFilter> _logger;
    
    public GeneralHubFilter(DatabaseContext context, ILogger<GeneralHubFilter> logger)
    {
        _context = context;
        _logger = logger;
    }
    public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        return next(context);
    }
}