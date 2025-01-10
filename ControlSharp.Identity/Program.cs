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
        private const string _configDataBaseName = "ControlSharpConfig.db";
        private const string _operationDataBaseName = "ControlSharpOperation.db";

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

            List<Client> AllClients = ClientGenerator.CreateClients(Builder.Configuration);
           
            Builder.Services.AddIdentityServer(options =>
            {
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryClients(AllClients)
            .AddInMemoryApiScopes(AllScopes.AppScopes)
            .AddInMemoryIdentityResources(AllIdentityResources.Resources)
            .AddInMemoryPersistedGrants()
            .AddInMemoryPushedAuthorizationRequests()
            .AddTestUsers(TestUsers.Users)
            .AddServerSideSessions();

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
            app.UseAuthorization();
            app.MapRazorPages().RequireAuthorization();

            app.UseHttpsRedirection();
            await app.RunAsync();
        }
    }
}
