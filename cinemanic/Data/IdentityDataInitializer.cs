using cinemanic.Models;
using Microsoft.AspNetCore.Identity;

namespace cinemanic.Data
{
    public static class IdentityDataInitializer
    {
        /// <summary>
        /// Seeds the initial identity data, including roles, using the specified <paramref name="userManager"/> and <paramref name="roleManager"/>.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="roleManager">The role manager.</param>
        public static async Task SeedData(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            if (await roleManager.FindByNameAsync(ApplicationRole.Admin) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(ApplicationRole.Admin));
            }

            if (await roleManager.FindByNameAsync(ApplicationRole.User) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(ApplicationRole.User));
            }
        }
    }
}
