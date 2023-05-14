using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemanic.Data
{
    public class TicketArchiveService : BackgroundService
    {
        private readonly CinemanicDbContext _dbContext;

        public TicketArchiveService(CinemanicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var today = DateTime.UtcNow.Date;
                var ticketsToArchive = await _dbContext.Tickets
                    .Include(t => t.Screening)
                    .Include(t => t.Order)
                    //    .ThenInclude(o => o.Account)
                    .Where(t => t.Screening.ScreeningDate.Date < today)
                    .ToListAsync(stoppingToken);

                // var userEmail = ticketsToArchive.FirstOrDefault().Order.Account.UserEmail;

                var archivedTickets = ticketsToArchive.Select(t => new ArchivedTicket
                {
                    //UserEmail = userEmail,
                    OrderId = t.OrderId,
                    Seat = t.Seat,
                    PricingType = t.PricingType,
                    TicketPrice = t.TicketPrice,
                    ScreeningId = t.ScreeningId,
                    Screening = t.Screening,
                    ScreeningDate = t.Screening.ScreeningDate,
                    ArchiveDate = today
                }).ToList();

                _dbContext.ArchivedTickets.AddRange(archivedTickets);
                _dbContext.Tickets.RemoveRange(ticketsToArchive);
                await _dbContext.SaveChangesAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
        public async Task ExecuteArchiveAsync(CancellationToken stoppingToken)
        {
            await ExecuteAsync(stoppingToken);
        }
    }
}
