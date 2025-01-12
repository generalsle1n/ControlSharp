using ControlSharp.Model.Identity.Role;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using ModelAsset = ControlSharp.Model.Database.Assets;

namespace ControlSharp.Ui.Controllers.Asset;

[Authorize(Policy = nameof(Roles.ControlSharpApi_Super_Read))]
public class AssetController : Controller
{
    private readonly ILogger<AssetController> _logger;
    private IHttpClientFactory _httpClientFactory;
    private const string _apiServerHostName = "ControlSharp-Api";

    public AssetController(ILogger<AssetController> logger, IHttpClientFactory httpClientFactory/*, SignInManager<User> signInManager*/)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    [Authorize(Policy = nameof(Roles.ControlSharpApi_Super_Read))]
    public async Task<IActionResult> Registered()
    {
        HttpClient Client = _httpClientFactory.CreateClient();

        HttpRequestMessage RequestMessage = await CreateAuthorizedRequestMessage();
        RequestMessage.Method = HttpMethod.Get;
        RequestMessage.RequestUri = new Uri($"https://{_apiServerHostName}/api/v0.1/Asset/Registered");
       
        HttpResponseMessage ResponseMessage = await Client.SendAsync(RequestMessage);

        List<ModelAsset.Asset> Result = new List<ModelAsset.Asset>();

        if (ResponseMessage.IsSuccessStatusCode)
        {
            List<ModelAsset.Asset> Data = await ResponseMessage.Content.ReadFromJsonAsync<List<ModelAsset.Asset>>();
            Result.AddRange(Data);
        }
        else
        {
            _logger.LogWarning($"{ResponseMessage.ReasonPhrase} To {ResponseMessage.RequestMessage.RequestUri.AbsolutePath}");
        }

        return View(Result);
    }

    [Authorize(Policy = nameof(Roles.ControlSharpApi_Super_Read))]
    public async Task<IActionResult> Quarantine()
    {
        HttpClient Client = _httpClientFactory.CreateClient();

        HttpRequestMessage RequestMessage = await CreateAuthorizedRequestMessage();
        RequestMessage.Method = HttpMethod.Get;
        RequestMessage.RequestUri = new Uri($"https://{_apiServerHostName}/api/v0.1/Asset/Unregistered");

        HttpResponseMessage ResponseMessage = await Client.SendAsync(RequestMessage);

        List<ModelAsset.Asset> Result = new List<ModelAsset.Asset>();

        if (ResponseMessage.IsSuccessStatusCode)
        {
            List<ModelAsset.Asset> Data = await ResponseMessage.Content.ReadFromJsonAsync<List<ModelAsset.Asset>>();
            IEnumerable<ModelAsset.Asset> NotBanned = Data.Where(asset => asset.Banned == false );
            Result.AddRange(NotBanned);
        }
        else
        {
            _logger.LogWarning($"{ResponseMessage.ReasonPhrase} To {ResponseMessage.RequestMessage.RequestUri.AbsolutePath}");
        }

        return View(Result);
    }

    [Authorize(Policy = nameof(Roles.ControlSharpApi_Super_Read))]
    public async Task<IActionResult> Banned()
    {
        HttpClient Client = _httpClientFactory.CreateClient();

        HttpRequestMessage RequestMessage = await CreateAuthorizedRequestMessage();
        RequestMessage.Method = HttpMethod.Get;
        RequestMessage.RequestUri = new Uri($"https://{_apiServerHostName}/api/v0.1/Asset/Unregistered");

        HttpResponseMessage ResponseMessage = await Client.SendAsync(RequestMessage);

        List<ModelAsset.Asset> Result = new List<ModelAsset.Asset>();

        if (ResponseMessage.IsSuccessStatusCode)
        {
            List<ModelAsset.Asset> Data = await ResponseMessage.Content.ReadFromJsonAsync<List<ModelAsset.Asset>>();
            IEnumerable<ModelAsset.Asset> NotBanned = Data.Where(asset => asset.Banned == true);
            Result.AddRange(NotBanned);
        }
        else
        {
            _logger.LogWarning($"{ResponseMessage.ReasonPhrase} To {ResponseMessage.RequestMessage.RequestUri.AbsolutePath}");
        }

        return View(Result);
    }

    public async Task<IActionResult> AddClient(ModelAsset.Asset Asset)
    {

        HttpClient Client = _httpClientFactory.CreateClient();

        HttpRequestMessage RequestMessage = await CreateAuthorizedRequestMessage();
        RequestMessage.Method = HttpMethod.Post;
        RequestMessage.RequestUri = new Uri($"https://{_apiServerHostName}/api/v0.1/Asset/{Asset.Id}");

        HttpResponseMessage AssetRegisterResponse = await Client.SendAsync(RequestMessage);

        if(!AssetRegisterResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning($"{AssetRegisterResponse.ReasonPhrase} To {AssetRegisterResponse.RequestMessage.RequestUri.AbsolutePath}");
        }

        return RedirectToAction("Registered");
    }

    public async Task<IActionResult> DeleteClient(ModelAsset.Asset Asset)
    {
        HttpClient Client = _httpClientFactory.CreateClient();

        HttpRequestMessage RequestMessage = await CreateAuthorizedRequestMessage();
        RequestMessage.Method = HttpMethod.Delete;
        RequestMessage.RequestUri = new Uri($"https://{_apiServerHostName}/api/v0.1/Asset/{Asset.Id}");

        HttpResponseMessage AssetDeleteResponse = await Client.SendAsync(RequestMessage);

        if (!AssetDeleteResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning($"{AssetDeleteResponse.ReasonPhrase} To {AssetDeleteResponse.RequestMessage.RequestUri.AbsolutePath}");
        }

        return RedirectToAction("Banned");
    }

    private async Task<HttpRequestMessage> CreateAuthorizedRequestMessage()
    {
        HttpRequestMessage Result = new HttpRequestMessage();

        string AccessToken = await HttpContext.GetTokenAsync("access_token");
        Result.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

        return Result;
    }
}