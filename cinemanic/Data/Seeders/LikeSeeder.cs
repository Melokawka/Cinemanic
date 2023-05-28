using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Data.Seeders
{
    public class LikeSeeder
    {
        /// <summary>
        /// Seeds likes in the database.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public static async Task SeedLikes(CinemanicDbContext dbContext)
        {
            var random = new Random();

            List<int> accountIds = await dbContext.Accounts.Select(a => a.Id).ToListAsync();
            List<int> movieIds = await dbContext.Movies.Select(a => a.Id).ToListAsync();

            for (int i = 0; i < random.Next(7, 10); i++)
            {
                var like = new Like
                {
                    AccountId = accountIds[random.Next(accountIds.Count)],
                    MovieId = movieIds[random.Next(movieIds.Count)],
                };

                dbContext.Likes.AddRange(like);
            }

            dbContext.SaveChanges();
        }
    }
}
