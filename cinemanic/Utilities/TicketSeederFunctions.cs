using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Data
{
    /// <summary>
    /// Utility class for ticket seeder functions.
    /// </summary>
    public static class TicketSeederFunctions
    {
        /// <summary>
        /// Saves the provided tickets to the database.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="tickets">The list of tickets to save.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task SaveTickets(CinemanicDbContext dbContext, List<Ticket> tickets)
        {
            await dbContext.Tickets.AddRangeAsync(tickets);
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the screening IDs from the database.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <returns>A task representing the asynchronous operation. The list of screening IDs.</returns>
        public static async Task<List<int>> GetScreeningIds(CinemanicDbContext dbContext)
        {
            return await dbContext.Screenings.Select(a => a.Id).ToListAsync();
        }

        /// <summary>
        /// Gets the account IDs from the database.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <returns>A task representing the asynchronous operation. The list of account IDs.</returns>
        public static async Task<List<int>> GetAccountIds(CinemanicDbContext dbContext)
        {
            return await dbContext.Accounts.Select(a => a.Id).ToListAsync();
        }

        /// <summary>
        /// Gets the order IDs from the database.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <returns>A task representing the asynchronous operation. The list of order IDs.</returns>
        public static async Task<List<int>> GetOrderIds(CinemanicDbContext dbContext)
        {
            return await dbContext.Orders.Select(a => a.Id).ToListAsync();
        }

        /// <summary>
        /// Gets the room ID for the provided screening ID.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="screeningId">The screening ID.</param>
        /// <returns>A task representing the asynchronous operation. The room ID.</returns>
        public static async Task<int> GetRoomIdForScreening(CinemanicDbContext dbContext, int screeningId)
        {
            return await dbContext.Screenings.Where(s => s.Id == screeningId).Select(s => s.RoomId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets the number of seats for the provided room ID.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="roomId">The room ID.</param>
        /// <returns>A task representing the asynchronous operation. The number of seats.</returns>
        public static async Task<int> GetSeatsForRoom(CinemanicDbContext dbContext, int roomId)
        {
            return await dbContext.Rooms.Where(r => r.Id == roomId).Select(r => r.Seats).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets the future screening IDs from the database.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <returns>A task representing the asynchronous operation. The list of future screening IDs.</returns>
        public static async Task<List<int>> GetFutureScreeningIds(CinemanicDbContext dbContext)
        {
            var now = DateTime.Now;
            return await dbContext.Screenings.Where(s => s.ScreeningDate > now).Select(s => s.Id).ToListAsync();
        }
    }
}
