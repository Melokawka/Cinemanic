using cinemanic.Data;
using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

public class TicketArchiveHostedService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceProvider _serviceProvider;

    public TicketArchiveHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.FromMinutes(1), TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CinemanicDbContext>();
            var today = DateTime.Now;
            var ticketsToArchive = await dbContext.Tickets
                .Include(t => t.Screening)
                .Include(t => t.Order)
                .Where(t => t.Screening.ScreeningDate.Date < today)
                .ToListAsync();

            var archivedTickets = ticketsToArchive.Select(t => new ArchivedTicket
            {
                OrderId = t.OrderId,
                Seat = t.Seat,
                PricingType = t.PricingType,
                TicketPrice = t.TicketPrice,
                IsActive = false,
                ScreeningId = t.ScreeningId,
                Screening = t.Screening,
                ScreeningDate = t.Screening.ScreeningDate,
                ArchiveDate = today
            }).ToList();

            dbContext.ArchivedTickets.AddRange(archivedTickets);
            dbContext.Tickets.RemoveRange(ticketsToArchive);
            await dbContext.SaveChangesAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
