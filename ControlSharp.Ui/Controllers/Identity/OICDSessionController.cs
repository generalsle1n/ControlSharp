using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using ControlSharp.Ui.Models;

namespace ControlSharp.Ui.Controllers.Identity
{
    [Authorize]
    public class OICDSessionController : Controller
    {
        public OICDSessionController()
        {

        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> LogoffAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> UserProfileAsync()
        {
            AuthenticateResult AuthResult = await HttpContext.AuthenticateAsync();

            UserProperty Result = new UserProperty();
            
            foreach(Claim SingleClaim in AuthResult.Principal.Claims)
            {
                Result.Claims.Add(new KeyValuePair<string, string>(SingleClaim.Type, SingleClaim.Value));
            }

            foreach(var SingleProperty in AuthResult.Properties.Items)
            {
                Result.Properties.Add(new KeyValuePair<string, string>(SingleProperty.Key, SingleProperty.Value));
            }

            return View(Result);
        }

    }
}
