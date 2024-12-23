using ControlSharp.Database.Identity;
using ControlSharp.Database.Identity.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddDatabase();
//Aspire
builder.AddServiceDefaults();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(option =>
{
    
});

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy(AccessRole.Super.ToString(), policy =>
    {
        policy.RequireRole(AccessRole.Super.ToString());
    });
});

builder.Services.AddDefaultIdentity<User>()
    .AddSignInManager<SignInManager<User>>()
    .AddRoles<Role>()
    .AddEntityFrameworkStores<DatabaseContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();
app.MapIdentityApi<User>();
app.InitializeDatabase();
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
app.MapRazorPages()
    .WithStaticAssets();
app.Run();