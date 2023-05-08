using Bogus;
using cinemanic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace cinemanic.Data
{
    public class TicketSeeder
    {
        public static async Task SeedTickets(CinemanicDbContext dbContext)
        {
            var random = new Random();

            List<int> screeningIds = await dbContext.Screenings.Select(a => a.Id).ToListAsync();

            List<int> accountIds = await dbContext.Accounts.Select(a => a.Id).ToListAsync();

            List<int> orderIds = await dbContext.Orders.Select(a => a.Id).ToListAsync();

            for (int i = 0; i < random.Next(40, 50); i++)
            {
                int randomScreeningId = screeningIds[random.Next(screeningIds.Count)];

                int roomId = await dbContext.Screenings.Where(s => s.Id == randomScreeningId).Select(s => s.RoomId).FirstOrDefaultAsync();
                int seats = await dbContext.Rooms.Where(r => r.Id == roomId).Select(r => r.Seats).FirstOrDefaultAsync();

                int randomSeat = random.Next(1, seats + 1);
                while (dbContext.Tickets.Local.Any(t => t.ScreeningId == randomScreeningId && t.Seat == randomSeat))
                {
                    randomSeat = random.Next(1, seats + 1);
                }

                var randomPricingType = Convert.ToBoolean(random.Next(2)) ? PricingType.NORMALNY : PricingType.ULGOWY;

                var ticket = new Ticket
                {
                    Seat = randomSeat,
                    PricingType = randomPricingType,
                    TicketPrice = (randomPricingType == PricingType.ULGOWY) ? (decimal)(random.Next(1100, 2300) * 0.01 * 0.5)  : (decimal)(random.Next(1100, 2300) * 0.01),
                    ScreeningId = randomScreeningId,
                    OrderId = orderIds[random.Next(orderIds.Count)],
                };

                dbContext.Tickets.AddRange(ticket);

                ticket.Order.TotalPrice += ticket.TicketPrice;
            }

            dbContext.SaveChanges();
        }
    }
}
