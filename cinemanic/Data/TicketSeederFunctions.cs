using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Data
{
    public static class TicketSeederFunctions
    {
        public static async Task SaveTickets(CinemanicDbContext dbContext, List<Ticket> tickets)
        {
            await dbContext.Tickets.AddRangeAsync(tickets);
            await dbContext.SaveChangesAsync();
        }

        public static async Task<List<int>> GetScreeningIds(CinemanicDbContext dbContext)
        {
            return await dbContext.Screenings.Select(a => a.Id).ToListAsync();
        }

        public static async Task<List<int>> GetAccountIds(CinemanicDbContext dbContext)
        {
            return await dbContext.Accounts.Select(a => a.Id).ToListAsync();
        }

        public static async Task<List<int>> GetOrderIds(CinemanicDbContext dbContext)
        {
            return await dbContext.Orders.Select(a => a.Id).ToListAsync();
        }

        public static async Task<int> GetRoomIdForScreening(CinemanicDbContext dbContext, int screeningId)
        {
            return await dbContext.Screenings.Where(s => s.Id == screeningId).Select(s => s.RoomId).FirstOrDefaultAsync();
        }

        public static async Task<int> GetSeatsForRoom(CinemanicDbContext dbContext, int roomId)
        {
            return await dbContext.Rooms.Where(r => r.Id == roomId).Select(r => r.Seats).FirstOrDefaultAsync();
        }

        public static async Task<List<int>> GetFutureScreeningIds(CinemanicDbContext dbContext)
        {
            var now = DateTime.Now;
            return await dbContext.Screenings.Where(s => s.ScreeningDate > now).Select(s => s.Id).ToListAsync();
        }
    }
}
