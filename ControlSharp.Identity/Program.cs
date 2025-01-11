using ControlSharp.Identity.Config.Clients;
using ControlSharp.Identity.Config.IdentityResourcesConfig;
using ControlSharp.Identity.Config.Scopes;
using ControlSharp.Identity.Identity.Database;
using ControlSharp.Identity.Identity.User;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ControlSharp.Identity
{
    public class Program
    {
        private const string _configDataBaseName = "Identity.db";

        public static async Task Main(string[] args)
        {
            WebApplicationBuilder Builder = WebApplication.CreateBuilder(args);

            //For Aspire
            Builder.AddServiceDefaults();

            //For Swagger
            Builder.Services.AddEndpointsApiExplorer();
            Builder.Services.AddSwaggerGen();

            //IdentityServer
            // uncomment if you want to add a UI
            Builder.Services.AddRazorPages();

            Builder.Services.AddDbContext<IdentityDatabaseContext>(option =>
            {
                string FolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string DatabasePath = Path.Combine(FolderPath, _configDataBaseName);

                option.UseSqlite($"DataSource={DatabasePath};Cache=Shared");
            });

            Builder.Services.AddIdentity<UserIdentity, IdentityRole>(option =>
            {
                option.Password.RequiredLength = 10;
                option.Password.RequireNonAlphanumeric = true;
                option.Password.RequireLowercase = true;
                option.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<IdentityDatabaseContext>()
            .AddDefaultTokenProviders();
            
            List<Client> AllClients = ClientGenerator.CreateClients(Builder.Configuration);
           
            Builder.Services.AddIdentityServer(options =>
            {
                options.EmitStaticAudienceClaim = true;

                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddInMemoryClients(AllClients)
            .AddInMemoryApiScopes(AllScopes.AppScopes)
            .AddInMemoryIdentityResources(AllIdentityResources.Resources)
            .AddInMemoryPersistedGrants()
            .AddInMemoryPushedAuthorizationRequests()
            .AddServerSideSessions()
            .AddAspNetIdentity<UserIdentity>();

            WebApplication app = Builder.Build();

            app.MapDefaultEndpoints();

            //CreateDatabase
            await app.GenerateDatabase();

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
            app.UseAuthorization();
            app.MapRazorPages().RequireAuthorization();

            app.UseHttpsRedirection();
            await app.RunAsync();
        }
    }
}
