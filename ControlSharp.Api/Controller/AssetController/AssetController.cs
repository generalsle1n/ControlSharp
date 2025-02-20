using ControlSharp.Api.Database;
using ControlSharp.Api.Hubs;
using ControlSharp.Api.Hubs.Interfaces;
using ControlSharp.Model.Database.Assets;
using ControlSharp.Model.Identity.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ControlSharp.Api.Controller.AssetController;

[ApiController]
[Route("api/v0.1/[controller]")]
[Produces("application/json")]
[Authorize(Policy = nameof(Roles.ControlSharpApi_Super_Read))]
public class AssetController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly ILogger<AssetController> _logger;
    private readonly IHubContext<QuarantineAssetHub, IQuarantineAssetAction> _quarantineAssetHub;
    private readonly IHubContext<RegisteredAssetHub, IRegisteredAssetClient> _registeredAssetHub;
    
    public AssetController(DatabaseContext context, ILogger<AssetController> logger, IHubContext<QuarantineAssetHub, IQuarantineAssetAction> quarantineAssetHub, IHubContext<RegisteredAssetHub, IRegisteredAssetClient> registeredAssetHub)
    {
        _context = context;
        _logger = logger;
        _quarantineAssetHub = quarantineAssetHub;
        _registeredAssetHub = registeredAssetHub;
    }
    
    [HttpGet]
    [Authorize(Policy = nameof(Roles.ControlSharpApi_Super_Read))]
    [Route("[action]")]
    public async Task<ActionResult<List<Asset>>> Registered()
    {
        return _context.Assets.Where(asset => asset.Registered == true).ToList();
    }
    
    [HttpGet]
    [Authorize(Policy = nameof(Roles.ControlSharpApi_Super_Write))]
    [Route("[action]")]
    public async Task<ActionResult<List<Asset>>> Unregistered()
    {
        return _context.Assets.Where(asset => asset.Registered == false).ToList();
    }

    [HttpGet]
    [Authorize(Policy = nameof(Roles.ControlSharpApi_Super_Read))]
    [Route("{ID}")]
    public async Task<ActionResult<Asset>> GetAsset(Guid ID)
    {
        Asset asset = await _context.Assets.FindAsync(ID);
        if (asset is not null)
        {
            return Ok(asset);
        }
        else
        {
            _logger.LogWarning($"Asset requested but not exist ({ID})");
            return NotFound();
        }
    }
    
    [HttpPost]
    [Authorize(Policy = nameof(Roles.ControlSharpApi_Super_Write))]
    [Route("{ID}")]
    public async Task<ActionResult> CreateAsset(Guid ID, CancellationToken token)
    {
        try
        {
            Asset asset = await _context.Assets.FindAsync(ID);
            if (asset is not null)
            {
                asset.Registered = true;
                asset.Banned = false;
                await _context.SaveChangesAsync(token);
                _logger.LogInformation($"Asset registerd {asset.Id}");
                return Ok(asset);
            }
            else
            {
                _logger.LogWarning($"Requested to Register asset {ID} but not found");
                return NotFound();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to register Asset");
            return StatusCode(500);
        }
    }
    
    [HttpDelete]
    [Authorize(Policy = nameof(Roles.ControlSharpApi_Super_Write))]
    [Route("{ID}")]
    public async Task<ActionResult> DeleteAsset(Guid ID, CancellationToken token)
    {
        try
        {
            Asset asset = await _context.Assets.FindAsync(ID);
            if (asset is not null)
            {
                if (asset.Banned != true)
                {
                    asset.Registered = false;
                    asset.Banned = true;
                    _context.Assets.Update(asset);
                    await _context.SaveChangesAsync(token);
                }
                
                IQuarantineAssetAction QuarantineClient = _quarantineAssetHub.Clients.Client(asset.ConnectionId);
                IRegisteredAssetClient RegisteredClient = _registeredAssetHub.Clients.Client(asset.ConnectionId);

                await QuarantineClient.DestroyAssetAsync();
                await RegisteredClient.DestroyAssetAsync();

                _logger.LogInformation($"Deleted Asset {asset.Id}");

                return Ok(asset);
            }
            else
            {
                _logger.LogWarning($"Requested to delete Asset ({ID}) but it dont exist");
                return NotFound();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to delete Asset");
            return StatusCode(500);
        }
    }
}