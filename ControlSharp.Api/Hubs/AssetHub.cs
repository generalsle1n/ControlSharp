using ControlSharp.Api.Hubs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ControlSharp.Api.Hubs;

[Authorize]
public class AssetHub : Hub
{
    public async Task SendMessageToAll(string message)
    {
        await Clients.All.SendAsync("cool", message);
    }
    

}