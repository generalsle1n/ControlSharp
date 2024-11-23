using System.Reflection;
using ControlSharp.Api.Hubs;
using ControlSharp.Api.Hubs.Filter;
using ControlSharp.Database.Identity;
using ControlSharp.Database.Identity.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
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
Builder.Services.AddSignalR(option =>
    {
        option.AddFilter<GeneralHubFilter>();
    })
    .AddMessagePackProtocol();

Builder.Configuration.AddInMemoryCollection(new List<KeyValuePair<string, string?>>()
{
    new KeyValuePair<string, string?>("AssetHubId", $"{Guid.NewGuid()}{Guid.NewGuid()}")
});

// Builder.Services.AddSerilog(Config =>
// {
//     string FilePath = Assembly.GetExecutingAssembly().Location;
//     string FolderPath = $@"{Path.GetDirectoryName(FilePath)}\Log.txt";
//
//     Config.MinimumLevel.Debug();
//     
//     Config.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information);
//     Config.WriteTo.File(path: FolderPath);
// });
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
app.InitializeDatabase(CreateDatabase: true);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<QuarantineAssetHub>("/login", config =>
{
    config.AllowStatefulReconnects = true;
});
app.MapHub<RegisteredAssetHub>($"/{app.Configuration.GetValue<string>("AssetHubId")}", config =>
{
    config.AllowStatefulReconnects = true;
    config.CloseOnAuthenticationExpiration = true;
});

app.MapControllers();

app.Run();
