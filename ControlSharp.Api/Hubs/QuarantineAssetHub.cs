using ControlSharp.Api.Hubs.Interfaces;
using ControlSharp.Database.Identity;
using ControlSharp.Database.Identity.Model;
using Microsoft.AspNetCore.SignalR;

namespace ControlSharp.Api.Hubs;

public class QuarantineAssetHub : Hub<IQuarantineAssetHub>
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
        IQueryable<Asset> Result = _context.Asset.Where(asset => asset.Hash.Equals(Hash) && asset.Registered == true);
        if (Result.Count() == 1)
        {
            IQuarantineAssetHub SingleClient = Clients.Client(Context.ConnectionId);
            SingleClient.CreateConnectingToMain(_configuration.GetValue<string>("AssetHubId"));
        }
        else
        {
            await _context.Asset.AddAsync(new Asset()
            {
                Id = Guid.NewGuid(),
                Created = DateTimeOffset.Now,
                Name = Hostname,
                Hash = Hash,
                Registered = false
            });
            
            await _context.SaveChangesAsync();
        }
    } 
}