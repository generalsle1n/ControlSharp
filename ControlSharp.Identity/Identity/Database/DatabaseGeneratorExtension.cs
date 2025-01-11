using ControlSharp.Identity.Identity.CustomClaim;
using ControlSharp.Identity.Identity.User;
using ControlSharp.Model.Identity.Role;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Security.Claims;

namespace ControlSharp.Identity.Identity.Database
{
    public static class DatabaseGeneratorExtension
    {
        private const string _adminUserName = "admin";
        public static async Task GenerateDatabase(this WebApplication App)
        {
            using(IServiceScope Scope = App.Services.CreateScope())
            {
                IdentityDatabaseContext DatabaseContext = Scope.ServiceProvider.GetRequiredService<IdentityDatabaseContext>();
                App.Logger.LogInformation("Check if Database is found");

                await DatabaseContext.Database.EnsureDeletedAsync();
                bool Created = await DatabaseContext.Database.EnsureCreatedAsync();

                if (Created)
                {
                    App.Logger.LogInformation("Database was created");
                    using(UserManager<UserIdentity> UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>())
                    {
                        UserIdentity Admin = new UserIdentity()
                        {
                            UserName = _adminUserName
                        };

                        string DefaultPassword = Guid.NewGuid().ToString();
                        
                        IdentityResult Result = await UserManager.CreateAsync(Admin, DefaultPassword);

                        if (Result.Succeeded)
                        {
                            App.Logger.LogInformation($"Admin Created: {_adminUserName} - {DefaultPassword}");

                            using(RoleManager<IdentityRole> RoleManager = Scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>())
                            using(App.Logger.BeginScope("RoleCreation"))
                            {
                                Array AllRoles = Enum.GetValues(typeof(Roles));
                                foreach(Roles SingleRole in AllRoles)
                                {
                                    IdentityRole Role = new IdentityRole()
                                    {
                                        Name = SingleRole.ToString()
                                    };

                                    IdentityResult RoleResult = await RoleManager.CreateAsync(Role);

                                    if (RoleResult.Succeeded)
                                    {
                                        App.Logger.LogInformation($"Created Role {Role.Name}");
                                    }
                                    else{
                                        foreach(IdentityError SingleError in RoleResult.Errors)
                                        {
                                            App.Logger.LogCritical($"Unable to create Role: {SingleError.Code}");
                                        }
                                    }
                                }
                            }

                            IdentityResult UserRoleResult = await UserManager.AddToRoleAsync(Admin, nameof(Roles.ControlSharpApi_Super_Write));
                            
                            if (UserRoleResult.Succeeded)
                            {
                                App.Logger.LogInformation($"Added {Admin.UserName} to {nameof(Roles.ControlSharpApi_Super_Write)}");
                            }
                            else
                            {
                                foreach(IdentityError SingleError in UserRoleResult.Errors)
                                {
                                    App.Logger.LogCritical($"Unable to Add {Admin.UserName} to {nameof(Roles.ControlSharpApi_Super_Write)} because {SingleError.Code}");
                                }
                            }
                        }
                        else
                        {
                            foreach(IdentityError SingleError in Result.Errors)
                            {
                                App.Logger.LogCritical($"Creation failed because of: {SingleError.Code}");
                            }
                        }
                    }
                }
            }
        }
    }
}
