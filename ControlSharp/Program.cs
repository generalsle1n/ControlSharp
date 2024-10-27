using ControlSharp.Config;
using Microsoft.EntityFrameworkCore;

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
