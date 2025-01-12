using ControlSharp.Api.Database;
using ControlSharp.Api.Hubs;
using ControlSharp.Api.Hubs.Interfaces;
using ControlSharp.Model.Database.Assets;
using ControlSharp.Model.Identity.Role;
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
    
    public AssetController(DatabaseContext context, ILogger<AssetController> logger, IHubContext<QuarantineAssetHub, IQuarantineAssetAction> quarantineAssetHub, IHubContext<RegisteredAssetHub, IRegisteredAssetClient> registeredAssetHub)
    {
        _context = context;
        _logger = logger;
        _quarantineAssetHub = quarantineAssetHub;
        _registeredAssetHub = registeredAssetHub;
    }
    
    [HttpGet]
    [Authorize(Policy  = nameof(AccessRole.Super))]
    [Route("[action]")]
    public async Task<ActionResult<List<Asset>>> Registered()
    {
        return _context.Assets.Where(asset => asset.Registered == true).ToList();
    }
    
    [HttpGet]
    [Authorize(Policy  = nameof(AccessRole.Super))]
    [Route("[action]")]
    public async Task<ActionResult<List<Asset>>> Unregistered()
    {
        return _context.Assets.Where(asset => asset.Registered == false).ToList();
    }

    [HttpGet]
    [Authorize(Policy = nameof(AccessRole.Super))]
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
            Asset asset = await _context.Assets.FindAsync(ID);
            if (asset is not null)
            {
                asset.Registered = true;
                await _context.SaveChangesAsync(token);
                return Ok(asset);
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
            Asset asset = await _context.Assets.FindAsync(ID);
            if (asset is not null)
            {
                if (asset.Banned != true)
                {
                    asset.Banned = true;
                    _context.Assets.Update(asset);
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