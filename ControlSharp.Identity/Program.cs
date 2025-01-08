using ControlSharp.Identity.Config.Clients;
using ControlSharp.Identity.Config.IdentityResourcesConfig;
using ControlSharp.Identity.Config.Scopes;
using Duende.IdentityServer.Models;
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

            List<Client> AllClients = ClientGenerator.CreateClients(Builder.Configuration);
           
            Builder.Services.AddIdentityServer(options =>
            {
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryClients(AllClients)
            .AddInMemoryApiScopes(AllScopes.AppScopes)
            .AddInMemoryIdentityResources(AllIdentityResources.Resources);

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

            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
