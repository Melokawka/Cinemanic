using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Data.Seeders
{
    public class NewsletterClientSeeder
    {
        /// <summary>
        /// Seeds newsletter clients in the database.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public static async Task SeedNewsletterClients(CinemanicDbContext dbContext)
        {
            var random = new Random();

            List<int> accountIds = await dbContext.Accounts.Select(a => a.Id).ToListAsync();

            int number = random.Next(1, accountIds.Count);

            var randomAccountIds = GetRandomDistinctAccountIds(accountIds, number);

            randomAccountIds.Sort();

            var newsletterClients = randomAccountIds.Select(async accountId =>
            {
                var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);

                return new NewsletterClient
                {
                    AccountId = accountId,
                    Account = account,
                };
            });

            dbContext.NewsletterClients.AddRange(await Task.WhenAll(newsletterClients));

            dbContext.SaveChanges();
        }

        /// <summary>
        /// Generates a list of random and distinct account IDs.
        /// </summary>
        /// <param name="accountIds">The list of all account IDs.</param>
        /// <param name="count">The number of random account IDs to generate.</param>
        /// <returns>A list of random and distinct account IDs.</returns>
        private static List<int> GetRandomDistinctAccountIds(List<int> accountIds, int count)
        {
            var random = new Random();
            var randomAccountIds = new HashSet<int>();

            while (randomAccountIds.Count < count)
            {
                var index = random.Next(accountIds.Count);
                var accountId = accountIds[index];
                randomAccountIds.Add(accountId);
            }

            return randomAccountIds.ToList();
        }
    }
}
