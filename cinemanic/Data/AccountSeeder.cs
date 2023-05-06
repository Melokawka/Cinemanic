using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Seeders
{
    public static class AccountSeeder
    {
        public static void SeedAccounts(DbContext dbContext, int numberOfAccounts)
        {
            var random = new Random();

            for (int i = 0; i < numberOfAccounts; i++)
            {
                var account = new Account
                {
                    UserEmail = GenerateRandomEmail(),
                    Birthdate = GenerateRandomBirthdate(),
                    Password = GenerateRandomPassword()
                };

                dbContext.Set<Account>().Add(account);
            }

            dbContext.SaveChanges();
        }

        private static string GenerateRandomEmail()
        {
            // Logic to generate a random email address
            // Example implementation:
            return Guid.NewGuid().ToString() + "@example.com";
        }

        private static DateTime GenerateRandomBirthdate()
        {
            // Logic to generate a random birthdate within a desired range
            // Example implementation:
            var random = new Random();
            var startDateTime = new DateTime(1950, 1, 1);
            var endDateTime = DateTime.Now.Date;
            int range = (endDateTime - startDateTime).Days;
            return startDateTime.AddDays(random.Next(range));
        }

        private static string GenerateRandomPassword()
        {
            // Logic to generate a random password
            // Example implementation:
            return Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}
