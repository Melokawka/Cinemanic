using Bogus;
using cinemanic.Models;
using Microsoft.AspNetCore.Identity;

namespace cinemanic.Data.Seeders
{
    /// <summary>
    /// Provides methods to seed user accounts in the database.
    /// </summary>
    public static class AccountSeeder
    {
        /// <summary>
        /// Seeds admin and regular user accounts.
        /// </summary>
        /// <param name="userManager">The user manager instance.</param>
        /// <param name="dbContext">The database context.</param>
        public static async Task SeedAccounts(UserManager<ApplicationUser> userManager, CinemanicDbContext dbContext)
        {
            var adminEmail = "admin@example.com";
            var adminPassword = "1234";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser { Email = adminEmail, UserName = adminEmail, BirthDate = GenerateRandomBirthdate() };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, ApplicationRole.Admin);

                    var adminAccount = new Account
                    {
                        UserEmail = adminUser.Email,
                        Birthdate = adminUser.BirthDate,
                    };
                    dbContext.Accounts.Add(adminAccount);
                }
            }

            for (int i = 0; i < 5; i++)
            {
                var userEmail = "user" + i + "@example.com";
                var userPassword = "1234";
                if (await userManager.FindByEmailAsync(userEmail) == null)
                {
                    var user = new ApplicationUser { Email = userEmail, UserName = userEmail, BirthDate = GenerateRandomBirthdate() };
                    var result = await userManager.CreateAsync(user, userPassword);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, ApplicationRole.User);

                        var userAccount = new Account
                        {
                            UserEmail = user.Email,
                            Birthdate = user.BirthDate,
                        };
                        dbContext.Accounts.Add(userAccount);
                    }
                }
            }
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Generates a random email address using the Faker library.
        /// </summary>
        /// <returns>A random email address.</returns>
        private static string GenerateRandomEmail()
        {
            var faker = new Faker("pl");
            return faker.Internet.Email();
        }

        /// <summary>
        /// Generates a random birthdate within a specified range.
        /// </summary>
        /// <returns>A random birthdate.</returns>
        private static DateTime GenerateRandomBirthdate()
        {
            var random = new Random();

            var startDateTime = new DateTime(1950, 1, 1);
            var endDateTime = DateTime.Now.Date;

            int range = (endDateTime - startDateTime).Days;
            return startDateTime.AddDays(random.Next(range));
        }
    }
}
