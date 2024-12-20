using System.Reflection;
using ControlSharp.Api.Database;
using ControlSharp.Api.Database.Model;
using ControlSharp.Api.Extension;
using ControlSharp.Api.Services;
using Microsoft.EntityFrameworkCore;
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

app.MapControllers();

app.Run();
