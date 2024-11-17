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

    [HttpPost]
    [Authorize(Policy = nameof(AccessRole.Super))]
    [Route("[action]")]
    public async Task<ActionResult> RegisterAsset(Asset asset, CancellationToken token)
    {
        try
        {
            asset.Registered = true;
            _context.Asset.Update(asset);
            await _context.SaveChangesAsync(token);
            return Ok();
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e);
        }
    }
}