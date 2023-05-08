using AutoMapper.QueryableExtensions;
using cinemanic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace cinemanic.Data
{
    public class NewsletterClientSeeder
    {
        public static async Task SeedNewsletterClients(CinemanicDbContext dbContext)
        {
            var random = new Random();

            List<int> accountIds = await dbContext.Accounts.Select(a => a.Id).ToListAsync();

            int number = random.Next(1, accountIds.Count);

            var randomAccountIds = new List<int>();

            for (int i = 0; i < number; i++)
            {
                var index = random.Next(accountIds.Count);
                var accountId = accountIds[index];

                while (randomAccountIds.Contains(accountId)) 
                {
                    index = random.Next(accountIds.Count);
                    accountId = accountIds[index];
                }
                randomAccountIds.Add(accountId);
            }

            randomAccountIds.Sort();

            for (int i = 0; i < number; i++)
            {
                var randomAccountId = randomAccountIds[i];

                var newsletterClient = new NewsletterClient
                {
                    AccountId = randomAccountId,
                    Account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == randomAccountId),
                };

                dbContext.NewsletterClients.AddRange(newsletterClient);
            }

            dbContext.SaveChanges();
        }
    }
}
