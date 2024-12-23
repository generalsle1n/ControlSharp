using ControlSharp.Api.Hubs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ControlSharp.Api.Hubs;

[Authorize]
public class RegisteredAssetHub : Hub<IRegisteredAssetClient>
{
    public async Task SendMessageToAllaaaa(string message)
    {
        await Clients.All.ExecuteBinary(message);
    }
    

}