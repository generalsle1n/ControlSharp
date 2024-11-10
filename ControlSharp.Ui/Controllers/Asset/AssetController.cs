using System.ComponentModel;
using ControlSharp.Database.Identity.Model;
using ControlSharp.Ui.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Controllers.Asset;

public class AssetController : Controller
{
    private readonly ILogger<AssetController> _logger;
    private IHttpClientFactory _httpClientFactory;
    private SignInManager<User> _signInManager;
    public AssetController(ILogger<AssetController> logger, IHttpClientFactory httpClientFactory, SignInManager<User> signInManager)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _signInManager = signInManager;
    }
    
    public async Task<IActionResult> Asset()
    {
        return View();
    }
}