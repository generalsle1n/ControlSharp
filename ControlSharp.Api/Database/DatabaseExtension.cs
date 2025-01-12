namespace ControlSharp.Api.Database
{
    internal static class DatabaseExtension
    {
        internal static async Task CheckDatabaseAsync(this WebApplication webApplication)
        {
            using (IServiceScope SingleScope = webApplication.Services.CreateScope())
            using (DatabaseContext Context = SingleScope.ServiceProvider.GetRequiredService<DatabaseContext>())
            using (IDisposable LoggingScope = webApplication.Logger.BeginScope("Database Check"))
            {
                bool Created = await Context.Database.EnsureCreatedAsync();
                if (Created)
                {
                    webApplication.Logger.LogInformation("Database found");
                }
                else
                {
                    webApplication.Logger.LogInformation("Database created");
                }
            }
        }
    }
}