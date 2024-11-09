using ControlSharp.Database.Identity;
using ControlSharp.Database.Identity.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ControlSharp.Ui.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddDatabase();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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
    .AddRoles<Role>()
    .AddEntityFrameworkStores<DatabaseContext>();
// builder.Services.AddIdentity<User, Role>().AddEntityFrameworkStores<DatabaseContext>();
// builder.Services.AddIdentityApiEndpoints<User>()
//     .AddRoles<Role>()
//     .AddEntityFrameworkStores<DatabaseContext>();


builder.Services.AddControllersWithViews();

var app = builder.Build();
app.MapIdentityApi<User>();
app.InitializeDatabase();
// Configure the HTTP request pipeline.
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
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// app.MapRazorPages();

app.Run();