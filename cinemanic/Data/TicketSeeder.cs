using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

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

            List<Ticket> tickets = new();

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
                    TicketPrice = (randomPricingType == PricingType.ULGOWY) ? (decimal)(random.Next(1100, 2300) * 0.01 * 0.5) : (decimal)(random.Next(1100, 2300) * 0.01),
                    IsActive = true,
                    ScreeningId = randomScreeningId,
                    OrderId = orderIds[random.Next(orderIds.Count)],
                };
                tickets.Add(ticket);
            }

            if (!dbContext.Orders.Any(o => o.OrderStatus == OrderStatus.PENDING))
            {
                foreach (var account in accountIds)
                {
                    var order = new Order { AccountId = account, TotalPrice = 0, OrderStatus = OrderStatus.PENDING };
                    dbContext.Orders.Add(order);
                }
            }

            await dbContext.SaveChangesAsync();

            foreach (var account in accountIds)
            {
                var order = await dbContext.Orders.SingleAsync(o => o.AccountId == account && o.OrderStatus == OrderStatus.PENDING);

                Console.WriteLine("id konta " + account);

                for (var i = 0; i < 2; i++)
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
                        TicketPrice = (randomPricingType == PricingType.ULGOWY) ? (decimal)(random.Next(1100, 2300) * 0.01 * 0.5) : (decimal)(random.Next(1100, 2300) * 0.01),
                        IsActive = false,
                        ScreeningId = randomScreeningId,
                        OrderId = order.Id,
                        Order = order,
                    };
                    Console.WriteLine("cena " + ticket.TicketPrice);
                    Console.WriteLine("id zamowienia " + ticket.Order.Id);
                    tickets.Add(ticket);
                }
            }

            foreach (Ticket info in tickets) foreach (var property in typeof(Ticket).GetProperties()) Console.WriteLine(property.Name + " = " + property.GetValue(info));

            await dbContext.Tickets.AddRangeAsync(tickets);
            await dbContext.SaveChangesAsync();
        }
    }
}
