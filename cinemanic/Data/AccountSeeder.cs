using Bogus;
using cinemanic.Controllers;
using cinemanic.Data;
using cinemanic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;

namespace cinemanic.Seeders
{
    public static class AccountSeeder
    {
        public static Account SeedAccount(CinemanicDbContext dbContext)
        {
            var random = new Random();

            var account = new Account
            {
                UserEmail = GenerateRandomEmail(),
                Birthdate = GenerateRandomBirthdate(),
                Password = GenerateRandomPassword()
            };


            dbContext.Accounts.AddRange(account);

            dbContext.SaveChanges();

            return account;
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
            //return Guid.NewGuid().ToString().Substring(0, 8);
            using (var algorithm = SHA256.Create())
            {
                var hashedBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes("1234"));
                return BitConverter.ToString(hashedBytes).Replace("-", "");
            }
        }
    }
}
