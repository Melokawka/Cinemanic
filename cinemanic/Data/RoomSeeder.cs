using Bogus;
using cinemanic.Models;
using System.Text;

namespace cinemanic.Data
{
    public class RoomSeeder
    {
        public static void SeedRooms(CinemanicDbContext dbContext)
        {
            var random = new Random();

            for (int i = 0; i < random.Next(2,4); i++)
            {
                var room = new Room { Seats = random.Next(3,5) * 10 };

                dbContext.Rooms.AddRange(room);
            }
            
            dbContext.SaveChanges();
        }
    }
}
