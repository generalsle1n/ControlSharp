using ControlSharp.Model.Identity.Role;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.AddDatabase();

//Aspire
builder.AddServiceDefaults();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie("Cookies")
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = builder.Configuration.GetValue<string>("ControlSharpIdentityServer");
    
    options.ClientId = builder.Configuration.GetValue<string>("ControlSharpUiOICDId");
    options.ClientSecret = builder.Configuration.GetValue<string>("ControlSharpUiOICDSecret");
    options.ResponseType = "code";

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("roles");
    options.Scope.Add("ControlSharp-Api");

    options.ClaimActions.MapJsonKey("role", "role", "role");
    options.TokenValidationParameters.RoleClaimType = "role";

    options.GetClaimsFromUserInfoEndpoint = true;
    
    options.MapInboundClaims = false; // Don't rename claim types

    options.SaveTokens = true;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(nameof(Roles.ControlSharpApi_Super_Write), policy =>
    {
        policy.RequireRole(nameof(Roles.ControlSharpApi_Super_Write));
    });

    options.AddPolicy(nameof(Roles.ControlSharpApi_Super_Read), policy =>
    {
        policy.RequireRole(nameof(Roles.ControlSharpApi_Super_Read), nameof(Roles.ControlSharpApi_Super_Write));
    });

    options.AddPolicy(nameof(Roles.ControlSharpApi_Asset_Write), policy =>
    {
        policy.RequireRole(nameof(Roles.ControlSharpApi_Asset_Write));
    });

    options.AddPolicy(nameof(Roles.ControlSharpApi_Asset_Read), policy =>
    {
        policy.RequireRole(nameof(Roles.ControlSharpApi_Asset_Read));
    });
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

//Aspire
app.MapDefaultEndpoints();
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();
app.MapRazorPages()
    .WithStaticAssets();
app.Run();