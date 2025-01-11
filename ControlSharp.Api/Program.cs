using ControlSharp.Api.Database;
using ControlSharp.Api.Hubs.Filter;
using ControlSharp.Model.Identity.Role;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

const string _configDataBaseName = "Data.db";

WebApplicationBuilder Builder = WebApplication.CreateBuilder(args);

//For Aspire
Builder.AddServiceDefaults();

Builder.Services.AddControllers();
Builder.Services.AddEndpointsApiExplorer();
Builder.Services.AddSwaggerGen();
Builder.Services.AddSignalR(option =>
    {
        option.AddFilter<GeneralHubFilter>();
    })
    .AddMessagePackProtocol();
Builder.Services.AddDbContext<DatabaseContext>(options =>
{
    string FolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    string DatabasePath = Path.Combine(FolderPath, _configDataBaseName);

    options.UseSqlite($"DataSource={DatabasePath};Cache=Shared");
});


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

string Authority = Builder.Configuration.GetValue<string>("ControlSharpIdentityServer");
string ClientId = Builder.Configuration.GetValue<string>("ControlSharpApiOICDId");
string ClientSecret = Builder.Configuration.GetValue<string>("ControlSharpApiOICDSecret");

Builder.Services.AddAuthentication()
    .AddOAuth2Introspection(async options =>
{
        options.Authority = Authority;
        options.ClientId = ClientId;
        options.ClientSecret = ClientSecret;
    });
    
});

Builder.Services.AddAuthorization(option =>
{
    option.AddPolicy(AccessRole.Super.ToString(), policy =>
    {
        policy.RequireRole(AccessRole.Super.ToString());
    });
});

var app = Builder.Build();
await app.CheckDatabaseAsync();


app.MapDefaultEndpoints();

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

await app.RunAsync();
