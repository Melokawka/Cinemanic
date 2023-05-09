using Bogus;
using cinemanic.Data;
using cinemanic.Models;
using Microsoft.AspNetCore.Identity;

namespace cinemanic.Seeders
{
    public static class AccountSeeder
    {
        public static async Task SeedAccounts(UserManager<ApplicationUser> userManager, CinemanicDbContext dbContext)
        {
            // Create an admin user
            var adminEmail = "admin@example.com";
            var adminPassword = "1234";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser { Email = adminEmail, UserName = adminEmail };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, ApplicationRole.Admin);
                }
            }

            // Create a regular user
            for (int i = 0; i < 5; i++)
            {
                var userEmail = "user" + i + "@example.com";
                var userPassword = "1234";
                if (await userManager.FindByEmailAsync(userEmail) == null)
                {
                    var user = new ApplicationUser { Email = userEmail, UserName = userEmail, BirthDate = GenerateRandomBirthdate() };
                    var result = await userManager.CreateAsync(user, userPassword);
                    Console.WriteLine(user.Email.ToString());
                    Console.WriteLine(user.UserName.ToString());
                    Console.WriteLine(user.BirthDate.ToString());
                    Console.WriteLine(result.Succeeded);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, ApplicationRole.User);
                        Console.WriteLine(user.Id.ToString());
                        Console.WriteLine(user.PasswordHash.ToString());

                        var userAccount = new Account
                        {
                            UserEmail = user.Email,
                            Birthdate = user.BirthDate,
                        };
                        dbContext.Accounts.Add(userAccount);

                        Console.WriteLine(userAccount.ToString());
                    }
                }
            }
            await dbContext.SaveChangesAsync();
        }

        private static string GenerateRandomEmail()
        {
            // Logic to generate a random email address
            var faker = new Faker("pl");
            return faker.Internet.Email();
        }

        private static DateTime GenerateRandomBirthdate()
        {
            // Logic to generate a random birthdate within a desired range
            var random = new Random();
            var startDateTime = new DateTime(1950, 1, 1);
            var endDateTime = DateTime.Now.Date;
            int range = (endDateTime - startDateTime).Days;
            return startDateTime.AddDays(random.Next(range));
        }
    }
}
