using ControlSharp.Database.Identity;
using ControlSharp.Database.Identity.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Api.Controller.AssetController;

[ApiController]
[Route("api/v0.1/[controller]")]
[Produces("application/json")]
// [MiddlewareFilter<ApiAuthFilter>]
public class AssetController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly ILogger<AssetController> _logger;
    
    public AssetController(DatabaseContext context, ILogger<AssetController> logger, UserManager<User>a)
    {
        _context = context;
        _logger = logger;
        
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
            Asset asset = await _context.Asset.FindAsync(ID);
            if (asset is not null)
            {
                asset.Registered = true;
                await _context.SaveChangesAsync(token);
                return Ok(asset);
            }
            return NotFound();
        }
        catch (Exception e)
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