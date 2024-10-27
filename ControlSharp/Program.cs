using System.Net.Sockets;
using System.Reflection;
using ControlSharp.Api.Config;
using ControlSharp.Api.Config.Model;
using ControlSharp.Api.Filter;
using ControlSharp.Api.Hubs;
using ControlSharp.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

const string DatabaseConnection = "DefaultConnection";

WebApplicationBuilder Builder = WebApplication.CreateBuilder(args);

Builder.AddServiceDefaults();

Builder.Services.AddControllers(option =>
{
    option.Filters.Add<ApiAuthFilter>();
});

Builder.Services.AddEndpointsApiExplorer();
Builder.Services.AddSwaggerGen();
Builder.Services.AddSignalR();


string ConnectionData = Builder.Configuration.GetConnectionString(DatabaseConnection);

Builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlite(ConnectionData);
});
Builder.Services.AddHostedService<MainControl>();

Builder.Services.AddSerilog(Config =>
{
    string FilePath = Assembly.GetExecutingAssembly().Location;
    string FolderPath = $@"{Path.GetDirectoryName(FilePath)}\Log.txt";

    Config.MinimumLevel.Debug();
    
    Config.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information);
    Config.WriteTo.File(path: FolderPath);
});

var app = Builder.Build();

app.MapDefaultEndpoints();
app.MapHub<AssetHub>("/api/0.1/assetHub");
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
