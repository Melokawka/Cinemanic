using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Data
{
    public class OrderSeeder
    {
        public static async Task SeedOrders(CinemanicDbContext dbContext)
        {
            var random = new Random();

            List<int> accountIds = await dbContext.Accounts.Select(a => a.Id).ToListAsync();

            for (int i = 0; i < random.Next(7, 14); i++)
            {
                var order = new Order
                {
                    TotalPrice = 0,
                    AccountId = accountIds[random.Next(accountIds.Count)],
                };

                dbContext.Orders.AddRange(order);
            }

            dbContext.SaveChanges();
        }
    }
}
