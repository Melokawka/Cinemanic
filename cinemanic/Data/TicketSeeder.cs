using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Data
{
    public class TicketSeeder
    {
        public static async Task SeedTickets(CinemanicDbContext dbContext)
        {
            var screeningIds = await TicketSeederFunctions.GetScreeningIds(dbContext);
            var accountIds = await TicketSeederFunctions.GetAccountIds(dbContext);
            var orderIds = await TicketSeederFunctions.GetOrderIds(dbContext);

            var tickets = await GenerateRandomTickets(screeningIds, orderIds, dbContext);
            await TicketSeederFunctions.SaveTickets(dbContext, tickets);

            await ClearPendingOrders(dbContext, accountIds);
            await CreatePendingOrders(dbContext, accountIds);
            tickets = await GenerateFutureTickets(dbContext, accountIds);

            await TicketSeederFunctions.SaveTickets(dbContext, tickets);
        }

        private async static Task<List<Ticket>> GenerateRandomTickets(List<int> screeningIds, List<int> orderIds, CinemanicDbContext dbContext)
        {
            var random = new Random();
            var tickets = new List<Ticket>();

            for (int i = 0; i < random.Next(40, 50); i++)
            {
                int randomScreeningId = screeningIds[random.Next(screeningIds.Count)];
                int roomId = await TicketSeederFunctions.GetRoomIdForScreening(dbContext, randomScreeningId);
                int seats = await TicketSeederFunctions.GetSeatsForRoom(dbContext, roomId);
                int randomSeat = GenerateUniqueRandomSeat(dbContext, randomScreeningId, seats);

                var randomPricingType = Convert.ToBoolean(random.Next(2)) ? PricingType.NORMALNY : PricingType.ULGOWY;

                var ticket = new Ticket
                {
                    Seat = randomSeat,
                    PricingType = randomPricingType,
                    TicketPrice = CalculateTicketPrice(randomPricingType),
                    IsActive = true,
                    ScreeningId = randomScreeningId,
                    OrderId = orderIds[random.Next(orderIds.Count)],
                };
                tickets.Add(ticket);
            }

            return tickets;
        }

        private static int GenerateUniqueRandomSeat(CinemanicDbContext dbContext, int screeningId, int seats)
        {
            var random = new Random();
            int randomSeat = random.Next(1, seats + 1);

            while (dbContext.Tickets.Local.Any(t => t.ScreeningId == screeningId && t.Seat == randomSeat))
            {
                randomSeat = random.Next(1, seats + 1);
            }

            return randomSeat;
        }

        private static decimal CalculateTicketPrice(PricingType pricingType)
        {
            var random = new Random();
            var basePrice = random.Next(1100, 2300) * 0.01;

            if (pricingType == PricingType.ULGOWY)
            {
                return (decimal)(basePrice * 0.5);
            }
            else
            {
                return (decimal)basePrice;
            }
        }

        private static async Task ClearPendingOrders(CinemanicDbContext dbContext, List<int> accountIds)
        {
            if (dbContext.Orders.Count(o => o.OrderStatus == OrderStatus.PENDING) < accountIds.Count)
            {
                var pendingOrders = dbContext.Orders.Where(o => o.OrderStatus == OrderStatus.PENDING);
                dbContext.Orders.RemoveRange(pendingOrders);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task CreatePendingOrders(CinemanicDbContext dbContext, List<int> accountIds)
        {
            foreach (var account in accountIds)
            {
                var order = new Order { AccountId = account, TotalPrice = 0, OrderStatus = OrderStatus.PENDING };
                dbContext.Orders.Add(order);
            }

            await dbContext.SaveChangesAsync();
        }

        private static async Task<List<Ticket>> GenerateFutureTickets(CinemanicDbContext dbContext, List<int> accountIds)
        {
            var random = new Random();
            var tickets = new List<Ticket>();

            foreach (var account in accountIds)
            {
                var order = await dbContext.Orders.SingleAsync(o => o.AccountId == account && o.OrderStatus == OrderStatus.PENDING);
                var futureScreeningIds = await TicketSeederFunctions.GetFutureScreeningIds(dbContext);

                for (var i = 0; i < 2; i++)
                {
                    int randomScreeningId = futureScreeningIds[random.Next(futureScreeningIds.Count)];
                    int roomId = await TicketSeederFunctions.GetRoomIdForScreening(dbContext, randomScreeningId);
                    int seats = await TicketSeederFunctions.GetSeatsForRoom(dbContext, roomId);
                    int randomSeat = GenerateUniqueRandomSeat(dbContext, randomScreeningId, seats);

                    var randomPricingType = Convert.ToBoolean(random.Next(2)) ? PricingType.NORMALNY : PricingType.ULGOWY;

                    var ticket = new Ticket
                    {
                        Seat = randomSeat,
                        PricingType = randomPricingType,
                        TicketPrice = CalculateTicketPrice(randomPricingType),
                        IsActive = false,
                        ScreeningId = randomScreeningId,
                        OrderId = order.Id,
                        Order = order,
                    };
                    tickets.Add(ticket);
                }
            }

            return tickets;
        }
    }
}
