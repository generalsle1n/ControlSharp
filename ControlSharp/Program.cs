using System.Net.Sockets;
using System.Reflection;
using ControlSharp.Api.Config;
using ControlSharp.Api.Config.Model;
using ControlSharp.Api.Filter;
using ControlSharp.Api.Hubs;
using ControlSharp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using Serilog;
using Serilog.Events;

const string DatabaseConnection = "DefaultConnection";

WebApplicationBuilder Builder = WebApplication.CreateBuilder(args);

//For Aspire
Builder.AddServiceDefaults();

// Builder.Services.AddControllers(option =>
// {
//     foreach (var filterMetadata in option.Filters)
//     {
//         Console.WriteLine();
//     }
//
//     option.Filters.Add<ApiAuthFilter>();
// });
Builder.Services.AddControllers();
Builder.Services.AddEndpointsApiExplorer();
Builder.Services.AddSwaggerGen();

string ConnectionData = Builder.Configuration.GetConnectionString(DatabaseConnection);

Builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlite(ConnectionData);
});

Builder.Services.AddSerilog(Config =>
{
    string FilePath = Assembly.GetExecutingAssembly().Location;
    string FolderPath = $@"{Path.GetDirectoryName(FilePath)}\Log.txt";

    Config.MinimumLevel.Debug();
    
    Config.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information);
    Config.WriteTo.File(path: FolderPath);
});

Builder.Services.AddAuthorization();
Builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<DatabaseContext>();

Builder.Services.Configure<IdentityOptions>(option =>
{
    option.Password.RequireDigit = true;
    option.Password.RequiredLength = 8;
    option.Password.RequireLowercase = true;
    option.Password.RequireUppercase = true;
    option.User.RequireUniqueEmail = true;
});

Builder.Services.AddHostedService<MainControl>();

var app = Builder.Build();

app.MapIdentityApi<User>();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
