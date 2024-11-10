using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Controllers.Asset;

public class AssetController : Controller
{
    public IActionResult Asset()
    private readonly ILogger<AssetController> _logger;
    private IHttpClientFactory _httpClientFactory;
    private SignInManager<User> _signInManager;
    public AssetController(ILogger<AssetController> logger, IHttpClientFactory httpClientFactory, SignInManager<User> signInManager)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _signInManager = signInManager;
    }
    
    {
        return View();
    }
}