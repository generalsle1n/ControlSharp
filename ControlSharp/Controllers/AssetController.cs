using ControlSharp.Api.Config;
using ControlSharp.Api.Config.Model;
using ControlSharp.Api.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Controllers;

[ApiController]
[Route("api/0.1/[controller]")]
[Produces("application/json")]
[MiddlewareFilter<ApiAuthFilter>]
public class AssetController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly ILogger<AssetController> _logger;
    
    public AssetController(DatabaseContext context, ILogger<AssetController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<Asset>>> GetAllAssets()
    {
        return _context.Asset.ToList();
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateNewAsset(Asset asset, CancellationToken token)
    {
        AccessRole Role = (HttpContext.Items["ApiKey"] as ApiKey).Role;

        if (Role == AccessRole.Admin)
        {
            bool Result = true;
            try
            {
                _context.Asset.Add(asset);
                await _context.SaveChangesAsync(token);

                return new OkResult();
            }
            catch (Exception e)
            {
                Result = false;
                return new BadRequestObjectResult(e);
            }
        }
        else
        {
            return new UnauthorizedResult();
        }
        
        
    }
}