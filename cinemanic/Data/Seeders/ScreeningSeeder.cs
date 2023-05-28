using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Data.Seeders
{
    public class ScreeningSeeder
    {
        /// <summary>
        /// Seeds screenings in the database.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public static async Task SeedScreenings(CinemanicDbContext dbContext)
        {
            var random = new Random();

            List<int> roomIds = await dbContext.Rooms.Select(a => a.Id).ToListAsync();

            List<int> movieIds = await dbContext.Movies.Select(a => a.Id).ToListAsync();

            for (int i = 0; i < random.Next(15, 20); i++)
            {
                int randomRoomId = roomIds[random.Next(roomIds.Count)];
                int seats = dbContext.Rooms.Where(r => r.Id == randomRoomId).Select(r => r.Seats).FirstOrDefault();

                var screening = new Screening
                {
                    ScreeningDate = GenerateRandomdate(),
                    Subtitles = Convert.ToBoolean(random.Next(2)),
                    Lector = Convert.ToBoolean(random.Next(2)),
                    Dubbing = Convert.ToBoolean(random.Next(2)),
                    Is3D = Convert.ToBoolean(random.Next(2)),
                    RoomId = randomRoomId,
                    MovieId = movieIds[random.Next(movieIds.Count)],
                    SeatsLeft = seats,
                };

                dbContext.Screenings.AddRange(screening);
            }

            dbContext.SaveChanges();
        }

        /// <summary>
        /// Generates a random screening date and time within a specific range.
        /// </summary>
        /// <returns>A randomly generated screening date and time.</returns>
        private static DateTime GenerateRandomdate()
        {
            var random = new Random();
            //var startDateTime = DateTime.Now.Date;
            //var endDateTime = new DateTime(2023, 12, 30);

            var now = DateTime.UtcNow;
            var minDate = DateTime.Now.Date.AddMonths(-6);
            var maxDate = DateTime.Now.Date.AddMonths(6);

            //int range = (endDateTime - startDateTime).Days;
            int range = (maxDate - minDate).Days;

            var randomTime = TimeSpan.FromMinutes(random.Next(10, 26) * 60 + random.Next(2) * 30);

            return minDate.AddDays(random.Next(range)) + randomTime;
        }
    }
}
