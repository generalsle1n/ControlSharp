using System.Net.Http.Headers;
using System.Text.Json;
using ControlSharp.Database.Identity.Model;
using ControlSharp.Ui.Models;
using Microsoft.AspNetCore.Identity;

namespace ControlSharp.Ui.Helper;

public class HttpRequestHelper
{
    internal static async Task<HttpRequestMessage> CreateRequestMessageWithApiAuthAsync(string url, HttpMethod method, string userMail, SignInManager<User> signInManager)
    {
        HttpRequestMessage Request = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new Uri(url)
        };
        
        string RawToken = await UserClaimHelper.GetClaimValueFromUser(userMail, UserClaimHelper.ApiClaimId, signInManager);
        ApiIdentity identity = JsonSerializer.Deserialize<ApiIdentity>(RawToken);
        Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", identity.AccessToken);
        
        return Request;
    }
}