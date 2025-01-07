using ControlSharp.Identity.Database;
using Microsoft.EntityFrameworkCore;

namespace ControlSharp.Identity
{
    public class Program
    {
        private const string _configDataBaseName = "ControlSharpConfig.db";
        private const string _operationDataBaseName = "ControlSharpOperation.db";

        public static void Main(string[] args)
        {
            WebApplicationBuilder Builder = WebApplication.CreateBuilder(args);

            //For Aspire
            Builder.AddServiceDefaults();

            //For Swagger
            Builder.Services.AddEndpointsApiExplorer();
            Builder.Services.AddSwaggerGen();

            //IdentityServer
            // uncomment if you want to add a UI
            //builder.Services.AddRazorPages();

            string ApplicationFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
           
            Builder.Services.AddIdentityServer(options =>
            {
                options.EmitStaticAudienceClaim = true;
            }).AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = (db) =>
                {
                    string DatabasePath = Path.Combine(ApplicationFolder, _configDataBaseName);
                    db.UseSqlite($"DataSource={DatabasePath};Cache=Shared");
                };
            }).AddOperationalStore(options =>
            {
                options.ConfigureDbContext = (db) =>
                {
                    string DatabasePath = Path.Combine(ApplicationFolder, _operationDataBaseName);
                    db.UseSqlite($"DataSource={DatabasePath};Cache=Shared");
                };
            });
            //.AddInMemoryIdentityResources(Config.IdentityResources)
            //.AddInMemoryApiScopes(Config.ApiScopes)
            //.AddInMemoryClients(Config.Clients);

            WebApplication app = Builder.Build();

            app.MapDefaultEndpoints();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            //For Identityserver
            //app.UseSerilogRequestLogging();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();

            // uncomment if you want to add a UI
            //app.UseAuthorization();
            //app.MapRazorPages().RequireAuthorization();

            DatabaseHelper DatabaseHelper = new DatabaseHelper();
            DatabaseHelper.CheckDatabase(app);

            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
