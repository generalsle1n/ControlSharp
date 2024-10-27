using ControlSharp.Api.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ControlSharp.Controllers;

public class RealtimeController : ControllerBase
{
    private readonly IHubContext<AssetHub> _hub;
    
    public RealtimeController(IHubContext<AssetHub> Hub)
    {
        _hub = Hub;
    }
}