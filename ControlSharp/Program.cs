using System.Reflection;
using ControlSharp.Config;
using ControlSharp.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

const string DatabaseConnection = "DefaultConnection";

WebApplicationBuilder Builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
Builder.Services.AddEndpointsApiExplorer();
Builder.Services.AddSwaggerGen();

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
