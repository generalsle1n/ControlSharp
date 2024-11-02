using Microsoft.AspNetCore.SignalR;

namespace ControlSharp.Api.Hubs;

public class AssetHub : Hub
{
    public async Task<string> WaitForMessage(string connectionId)
    {
        await Clients.Client(connectionId).SendAsync("aaaaa");
        return "bbbb";
    }
}