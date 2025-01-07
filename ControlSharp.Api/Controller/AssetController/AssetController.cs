using ControlSharp.Api.Hubs;
using ControlSharp.Api.Hubs.Interfaces;
using ControlSharp.Database;
using ControlSharp.Database.Identity;
using ControlSharp.Database.Identity.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ControlSharp.Api.Controller.AssetController;

[ApiController]
[Route("api/v0.1/[controller]")]
[Produces("application/json")]
[Authorize(Policy = nameof(AccessRole.Asset))]

public class AssetController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly ILogger<AssetController> _logger;
    private readonly IHubContext<QuarantineAssetHub, IQuarantineAssetAction> _quarantineAssetHub;
    private readonly IHubContext<RegisteredAssetHub, IRegisteredAssetClient> _registeredAssetHub;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    
    public AssetController(DatabaseContext context, ILogger<AssetController> logger, IHubContext<QuarantineAssetHub, IQuarantineAssetAction> quarantineAssetHub, IHubContext<RegisteredAssetHub, IRegisteredAssetClient> registeredAssetHub, UserManager<User> userManager, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _quarantineAssetHub = quarantineAssetHub;
        _registeredAssetHub = registeredAssetHub;
        _userManager = userManager;
        _configuration = configuration;
    }
    
    [HttpGet]
    [Authorize(Policy  = nameof(AccessRole.Super))]
    [Route("[action]")]
    public async Task<ActionResult<List<Asset>>> Registered()
    {
        return _context.Asset.Where(asset => asset.Registered == true).ToList();
    }
    
    [HttpGet]
    [Authorize(Policy  = nameof(AccessRole.Super))]
    [Route("[action]")]
    public async Task<ActionResult<List<Asset>>> Unregistered()
    {
        return _context.Asset.Where(asset => asset.Registered == false).ToList();
    }

    [HttpGet]
    [Authorize(Policy = nameof(AccessRole.Super))]
    [Route("{ID}")]
    public async Task<ActionResult<Asset>> GetAsset(Guid ID)
    {
        Asset asset = await _context.Asset.FindAsync(ID);
        if (asset is not null)
        {
            return Ok(asset);
        }
        else
        {
            return NotFound();
        }
    }
    
    [HttpPost]
    [Authorize(Policy = nameof(AccessRole.Super))]
    [Route("{ID}")]
    public async Task<ActionResult> CreateAsset(Guid ID, CancellationToken token)
    {
        try
        {
            Asset Asset = await _context.Asset.FindAsync(ID);
            if (Asset is not null)
            {
                Asset.Registered = true;
                await _context.SaveChangesAsync(token);
                
                _logger.LogInformation($"Asset registered: {Asset.Hash}");

                User AssetLogin = new User()
                {
                    UserName = Asset.Name,
                    Created = DateTimeOffset.Now,
                    Asset = Asset
                };

                string AssetPassword = Asset.Hash;

                await _userManager.CreateAsync(AssetLogin, AssetPassword);
                _logger.LogInformation($"Asset login created: {AssetLogin.Id}");

                await _userManager.AddToRoleAsync(AssetLogin, nameof(AccessRole.Asset));
                _logger.LogInformation($"Asset ({AssetLogin.Id}) added to role: {nameof(AccessRole.Asset)}");

                await _quarantineAssetHub.Clients.Client(Asset.ConnectionId).CreateConnectingToMain(_configuration.GetValue<string>("AssetHubId"));

                return Ok(Asset);
            }
            return NotFound();
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }
    
    [HttpDelete]
    [Authorize(Policy = nameof(AccessRole.Super))]
    [Route("{ID}")]
    public async Task<ActionResult> DeleteAsset(Guid ID, CancellationToken token)
    {
        try
        {
            Asset asset = await _context.Asset.FindAsync(ID);
            if (asset is not null)
            {
                if (asset.Banned != true)
                {
                    asset.Banned = true;
                    _context.Asset.Update(asset);
                    await _context.SaveChangesAsync(token);
                }
                
                IQuarantineAssetAction QuarantineClient = _quarantineAssetHub.Clients.Client(asset.ConnectionId);
                IRegisteredAssetClient RegisteredClient = _registeredAssetHub.Clients.Client(asset.ConnectionId);

                await QuarantineClient.DestroyAssetAsync();
                await RegisteredClient.DestroyAssetAsync();
                
                return Ok(asset);
            }
            return NotFound();
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }
}