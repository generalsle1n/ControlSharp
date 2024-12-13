using System.ComponentModel;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using ControlSharp.Database.Identity.Model;
using ControlSharp.Ui.Helper;
using ControlSharp.Ui.Models;
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
        HttpClient Client = _httpClientFactory.CreateClient();
        HttpRequestMessage Request = await HttpRequestHelper.CreateRequestMessageWithApiAuthAsync("https://ControlSharp-Api/api/0.1/Asset", HttpMethod.Get, User.Identity.Name, _signInManager);
        HttpResponseMessage Response = await Client.SendAsync(Request);
        
        List<Database.Identity.Model.Asset> Result = null;
        if (Response.IsSuccessStatusCode)
        {
            Result = await Response.Content.ReadFromJsonAsync<List<Database.Identity.Model.Asset>>();
        }
        
        return View(Result);
    }
}