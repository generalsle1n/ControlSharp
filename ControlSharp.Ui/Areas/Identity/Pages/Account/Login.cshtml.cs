// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using ControlSharp.Database.Identity.Model;
using ControlSharp.Ui.Helper;
using ControlSharp.Ui.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ControlSharp.Ui.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private IHttpClientFactory _clientFactory;

        public LoginModel(SignInManager<User> signInManager, ILogger<LoginModel> logger, IHttpClientFactory clientFactory)
        {
            _signInManager = signInManager;
            _logger = logger;
            _clientFactory = clientFactory;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in Ui.");
                    ApiIdentity Token = await LoginIntoApiAsync(Input);
                    if (Token.Success)
                    {
                        await AddApiIdentiyToUserClaimAsync(Input, Token);
                    }
                    

                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task<ApiIdentity> LoginIntoApiAsync(InputModel input)
        {
            HttpClient Client = _clientFactory.CreateClient();
            
            ApiIdentity Result = null;
            
            HttpRequestMessage RequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri("https://localhost:7175/login"));
            RequestMessage.Content = JsonContent.Create(Input);
            HttpResponseMessage ResponseMessage = await Client.SendAsync(RequestMessage);
            
            if (ResponseMessage.IsSuccessStatusCode)
            {
                Result = await ResponseMessage.Content.ReadFromJsonAsync<ApiIdentity>();
                Result.Success = true;
                _logger.LogInformation("Gather Api Token successfully.");
            }
            else
            {
                _logger.LogError("Gather Api Token failed.");
            }
            
            return Result;
        }

        private async Task AddApiIdentiyToUserClaimAsync(InputModel input, ApiIdentity apiIdentity)
        {
            _logger.LogDebug($"Search User in Manager: {Input.Email}");
            User User = await _signInManager.UserManager.FindByEmailAsync(Input.Email);

            await RemoveSingleClaimByTypeAsync(User, UserClaimHelper.ApiClaimId);
            
            string SerializedData = JsonSerializer.Serialize(apiIdentity);
            Claim Claim = new Claim(UserClaimHelper.ApiClaimId, SerializedData);
            await _signInManager.UserManager.AddClaimAsync(User, Claim);
        }

        private async Task RemoveSingleClaimByTypeAsync(User user, string type)
        {
            _logger.LogInformation($"Removing Claim from User: {user.Email}");
            _logger.LogDebug($"Searching all Claims from User: {user.Email}");
            IList<Claim> AllClaims = await _signInManager.UserManager.GetClaimsAsync(user);

            IEnumerable<Claim> Result = AllClaims.Where(claim => claim.Type.Equals(type));
            foreach (Claim Claim in Result)
            {
                _logger.LogDebug($"Removing Single Claim {Claim.Type} from User: {user.Email}");
                await _signInManager.UserManager.RemoveClaimAsync(user, Claim);
            }
        }
    }
}
