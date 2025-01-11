using System.ComponentModel;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using ControlSharp.Database.Identity.Model;
using ControlSharp.Model.Identity.Role;
using ControlSharp.Ui.Helper;
using ControlSharp.Ui.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Controllers.Asset;

[Authorize(Policy = nameof(Roles.ControlSharpApi_Super_Read))]
public class AssetController : Controller
{
    private readonly ILogger<AssetController> _logger;
    private IHttpClientFactory _httpClientFactory;

    public AssetController(ILogger<AssetController> logger, IHttpClientFactory httpClientFactory, SignInManager<User> signInManager)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<IActionResult> Registered()
    {
        HttpClient Client = _httpClientFactory.CreateClient();
        HttpRequestMessage Request = await HttpRequestHelper.CreateRequestMessageWithApiAuthAsync("https://ControlSharp-Api/api/v0.1/Asset/Registered", HttpMethod.Get, User.Identity.Name, _signInManager);
        HttpResponseMessage Response = await Client.SendAsync(Request);
        
        List<Database.Identity.Model.Asset> Result = null;
        if (Response.IsSuccessStatusCode)
        {
            Result = await Response.Content.ReadFromJsonAsync<List<Database.Identity.Model.Asset>>();
        }
        
        return View(Result);
    }
    public async Task<IActionResult> Quarantine()
    {
        HttpClient Client = _httpClientFactory.CreateClient();
        HttpRequestMessage Request = await HttpRequestHelper.CreateRequestMessageWithApiAuthAsync("https://ControlSharp-Api/api/v0.1/Asset/Unregistered", HttpMethod.Get, User.Identity.Name, _signInManager);
        HttpResponseMessage Response = await Client.SendAsync(Request);
        
        List<Database.Identity.Model.Asset> Result = null;
        if (Response.IsSuccessStatusCode)
        {
            Result = await Response.Content.ReadFromJsonAsync<List<Database.Identity.Model.Asset>>();
        }
        
        return View(Result);
    }

    public async Task<IActionResult> AddClient(Database.Identity.Model.Asset Asset)
    {
        HttpClient Client = _httpClientFactory.CreateClient();
 
        HttpRequestMessage AssetRegisterRequest = await HttpRequestHelper.CreateRequestMessageWithApiAuthAsync(
            $"https://ControlSharp-Api/api/v0.1/Asset/{Asset.Id}",
            HttpMethod.Post,
            User.Identity.Name,
            _signInManager);
        
        HttpResponseMessage AssetRegisterResponse = await Client.SendAsync(AssetRegisterRequest);
        
        return RedirectToAction("Quarantine");
    }
    public async Task<IActionResult> DeleteClient(Database.Identity.Model.Asset Asset)
    {
        HttpClient Client = _httpClientFactory.CreateClient();
        Client.Timeout = TimeSpan.FromMinutes(30);
        HttpRequestMessage AssetRegisterRequest = await HttpRequestHelper.CreateRequestMessageWithApiAuthAsync(
            $"https://ControlSharp-Api/api/v0.1/Asset/{Asset.Id}",
            HttpMethod.Delete,
            User.Identity.Name,
            _signInManager);
        
        HttpResponseMessage AssetRegisterResponse = await Client.SendAsync(AssetRegisterRequest);
        
        return RedirectToAction("Quarantine");
    }
}