using ControlSharp.Database.Identity;
using ControlSharp.Database.Identity.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class ExtensionDatabase
{
    public static IHostApplicationBuilder AddDatabase(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<DatabaseContext>(options =>
        {
            string ApplicationFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string DatabasePath = Path.Combine(ApplicationFolder, "Data.db"); 
            options.UseSqlite($"DataSource={DatabasePath};Cache=Shared");
        });

        return builder;
    }

    public static WebApplication InitializeDatabase(this WebApplication app, bool CreateDatabase)
    {
        IServiceScope Scope = app.Services.CreateScope();
        DatabaseContext Context = Scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        
        UserManager<User> UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        RoleManager<Role> roleManager = Scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        
        Task Result = Context.CheckDatabaseAsync(UserManager, roleManager, CreateDatabase);
        Result.Wait();
        
        return app;
    }
}