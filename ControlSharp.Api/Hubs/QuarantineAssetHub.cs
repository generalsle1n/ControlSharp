using ControlSharp.Api.Database;
using ControlSharp.Api.Hubs.Interfaces;
using ControlSharp.Model.Database.Assets;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ControlSharp.Api.Hubs;

public class QuarantineAssetHub : Hub<IQuarantineAssetAction>
{
    private readonly ILogger<QuarantineAssetHub> _logger;
    private readonly IConfiguration _configuration;
    private DatabaseContext _context;
    
    public QuarantineAssetHub(ILogger<QuarantineAssetHub> logger, IConfiguration configuration, DatabaseContext context)
    {
        _logger = logger;
        _configuration = configuration;
        _context = context;
    }
    
    public async Task Register(string Hash, string Hostname)
    {
        List<Asset> Result = await _context.Assets.Where(asset => asset.Hash.Equals(Hash)).ToListAsync();
        Asset Client = Result.FirstOrDefault();
        if (Client is not null)
        {
            if (Client.Registered == true)
            {
                IQuarantineAssetAction SingleClient = Clients.Client(Context.ConnectionId);
                SingleClient.CreateConnectingToMain(_configuration.GetValue<string>("AssetHubId"));
            }
            else if (Client.Banned == true)
            {
                IQuarantineAssetAction SingleClient = Clients.Client(Context.ConnectionId);
                SingleClient.DestroyAssetAsync();
                _logger.LogInformation($"Banned client tried to connect: {Client.Name} - {Client.Hash}");
            }
            else
            {
                string CurrentIp = Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
                
                if (!Client.Ip.Equals(CurrentIp))
                {
                    Client.Ip = CurrentIp;
                }
                
                Client.ConnectionId = Context.ConnectionId;
                Client.LastOnline = DateTimeOffset.Now;
                _context.Assets.Update(Client);
                await _context.SaveChangesAsync();
            }
        }else
        {
            await _context.Assets.AddAsync(new Asset()
            {
                Id = Guid.NewGuid(),
                Created = DateTimeOffset.Now,
                Name = Hostname,
                Hash = Hash,
                Registered = false,
                LastOnline = DateTimeOffset.Now,
                Ip = Context.GetHttpContext().Connection.RemoteIpAddress.ToString(),
                ConnectionId = Context.ConnectionId
            });
            
            await _context.SaveChangesAsync();
        }
    }
}