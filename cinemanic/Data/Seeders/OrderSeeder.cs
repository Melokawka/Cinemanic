using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Data.Seeders
{
    public class OrderSeeder
    {
        /// <summary>
        /// Seeds orders in the database.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
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
                    OrderStatus = OrderStatus.COMPLETED
                };

                dbContext.Orders.AddRange(order);
            }

            dbContext.SaveChanges();
        }
    }
}
