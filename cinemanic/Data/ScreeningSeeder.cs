using Bogus;
using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Data
{
    public class ScreeningSeeder
    {
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

        private static DateTime GenerateRandomdate()
        {
            var random = new Random();
            var startDateTime = DateTime.Now.Date;
            var endDateTime = new DateTime(2023, 12, 30);

            int range = (endDateTime - startDateTime).Days;

            var randomTime = TimeSpan.FromMinutes(random.Next(10, 26) * 60 + (random.Next(2) * 30));

            return startDateTime.AddDays(random.Next(range)) + randomTime;
        }
    }
}
