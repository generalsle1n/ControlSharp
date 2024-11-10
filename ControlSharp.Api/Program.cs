using System.Reflection;
using ControlSharp.Api.Hubs;
using ControlSharp.Database.Identity;
using ControlSharp.Database.Identity.Model;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Events;

const string DatabaseConnection = "DefaultConnection";

WebApplicationBuilder Builder = WebApplication.CreateBuilder(args);

//For Aspire
Builder.AddServiceDefaults();
Builder.AddDatabase();

Builder.Services.AddControllers();
Builder.Services.AddEndpointsApiExplorer();
Builder.Services.AddSwaggerGen();
Builder.Services.AddSignalR()
    .AddMessagePackProtocol();

Builder.Services.AddSerilog(Config =>
{
    string FilePath = Assembly.GetExecutingAssembly().Location;
    string FolderPath = $@"{Path.GetDirectoryName(FilePath)}\Log.txt";

    Config.MinimumLevel.Debug();
    
    Config.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information);
    Config.WriteTo.File(path: FolderPath);
});
Builder.Services.AddAuthentication(option =>
{
    
});

Builder.Services.AddAuthorization(option =>
{
    option.AddPolicy(AccessRole.Super.ToString(), policy =>
    {
        policy.RequireRole(AccessRole.Super.ToString());
    });
});
Builder.Services.AddIdentityApiEndpoints<User>()
    .AddSignInManager<SignInManager<User>>()
    .AddRoles<Role>()
    .AddEntityFrameworkStores<DatabaseContext>();

var app = Builder.Build();

app.MapIdentityApi<User>();

app.MapDefaultEndpoints();
app.InitializeDatabase();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<AssetHub>("/asset");

app.MapControllers();

app.Run();
