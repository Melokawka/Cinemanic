using cinemanic.Models;
using Microsoft.AspNetCore.Identity;

namespace cinemanic.Data
{
    public static class IdentityDataInitializer
    {
        public static async Task SeedData(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            // Create the admin role
            if (await roleManager.FindByNameAsync(ApplicationRole.Admin) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(ApplicationRole.Admin));
            }

            // Create the user role
            if (await roleManager.FindByNameAsync(ApplicationRole.User) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(ApplicationRole.User));
            }
        }
    }
}
