using Duende.IdentityServer.EntityFramework.DbContexts;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;

namespace ControlSharp.Identity.Database
{
    public class DatabaseHelper
    {
        internal void CheckDatabase(IApplicationBuilder App)
        {

            using (IServiceScope serviceScope = App.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                PersistedGrantDbContext PersitentContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                PersitentContext.Database.EnsureCreated();

                ConfigurationDbContext ConfigurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                ConfigurationContext.Database.EnsureCreated();
            }
                
            //if (!context.Clients.Any())
            //{
            //    foreach (var client in Config.Clients)
            //    {
            //        context.Clients.Add(client.ToEntity());
            //    }
            //    context.SaveChanges();
            //}

            //if (!context.IdentityResources.Any())
            //{
            //    foreach (var resource in Config.IdentityResources)
            //    {
            //        context.IdentityResources.Add(resource.ToEntity());
            //    }
            //    context.SaveChanges();
            //}

            //if (!context.ApiScopes.Any())
            //{
            //    foreach (var resource in Config.ApiScopes)
            //    {
            //        context.ApiScopes.Add(resource.ToEntity());
            //    }
            //    context.SaveChanges();
            //}
        }
    }
}