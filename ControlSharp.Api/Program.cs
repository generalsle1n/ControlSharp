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

MapApiFeatures Feature = new MapApiFeatures()
{
    Info = false,
    Login = true,
    Manage = false,
    Refresh = false,
    Register = false,
    ConfirmMail = false,
    ForgotPassword = false,
    ResetPassword = false,
    TwoFactor = false,
    ResendConfirmMail = false,
};

app.MapIdentityApiRaw<User>(Feature);

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

app.Run();
