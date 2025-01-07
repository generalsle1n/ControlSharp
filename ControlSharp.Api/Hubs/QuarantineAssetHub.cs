using ControlSharp.Api.Hubs.Interfaces;
using ControlSharp.Database.Identity;
using ControlSharp.Database.Identity.Model;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ControlSharp.Api.Hubs;

public class QuarantineAssetHub : Hub<IQuarantineAssetAction>
{
    private readonly ILogger<QuarantineAssetHub> _logger;
    private readonly IConfiguration _configuration;
    private DatabaseContext _context;
    private SignInManager<User> _signInManager;
    private UserManager<User> _userManager;
    private HttpClient _client;
    
    public QuarantineAssetHub(ILogger<QuarantineAssetHub> logger, IConfiguration configuration, DatabaseContext context, SignInManager<User> signInManager, UserManager<User> userManager, HttpClient client)
    {
        _logger = logger;
        _configuration = configuration;
        _context = context;
        _signInManager = signInManager;
        _userManager = userManager;
        _client = client;
    }
    
    public async Task Register(string Hash, string Hostname)
    {
        List<Asset> Result = await _context.Asset.Where(asset => asset.Hash.Equals(Hash)).ToListAsync();
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
                _context.Asset.Update(Client);
                await _context.SaveChangesAsync();
            }
        }else
        {
            await _context.Asset.AddAsync(new Asset()
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