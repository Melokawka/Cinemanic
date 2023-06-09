﻿using cinemanic.Data;
using cinemanic.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Hosted service for ticket archiving.
/// </summary>
public class TicketArchiveHostedService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceProvider _serviceProvider;
    private DateTime today;

    /// <summary>
    /// Initializes a new instance of the <see cref="TicketArchiveHostedService"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public TicketArchiveHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(15), TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    /// <summary>
    /// Performs the archive process for tickets and screenings.
    /// </summary>
    /// <param name="state">The state object passed to the timer.</param>
    private async void DoWork(object state)
    {
        today = DateTime.Now;
        List<Screening> screeningsToArchive = new();
        List<Ticket> ticketsToArchive = new();

        // Add to Archive
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CinemanicDbContext>();

            ticketsToArchive = await dbContext.Tickets
                .Include(t => t.Screening)
                .Include(t => t.Order)
                .Where(t => t.Screening.ScreeningDate.Date < today)
                .ToListAsync();

            screeningsToArchive = await dbContext.Screenings
                .Where(s => s.ScreeningDate < today)
                .ToListAsync();

            List<ArchivedTicket> archivedTickets = await PrepareArchivedTickets(ticketsToArchive);
            List<ArchivedScreening> archivedScreenings = await PrepareArchivedScreenings(screeningsToArchive, ticketsToArchive);

            // Dismiss tickets that werent bought
            dbContext.ArchivedTickets.AddRange(archivedTickets.Where(at => at.IsActive));
            dbContext.ArchivedScreenings.AddRange(archivedScreenings);

            await dbContext.SaveChangesAsync();
        }

        // Remove archived entries from the database
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CinemanicDbContext>();

            dbContext.Tickets.RemoveRange(ticketsToArchive);
            dbContext.Screenings.RemoveRange(screeningsToArchive);

            await dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Prepares a list of archived tickets based on the tickets to archive.
    /// </summary>
    /// <param name="ticketsToArchive">The list of tickets to archive.</param>
    /// <returns>A list of archived tickets.</returns>
    private async Task<List<ArchivedTicket>> PrepareArchivedTickets(List<Ticket> ticketsToArchive)
    {
        var archivedTickets = ticketsToArchive.Select(t => new ArchivedTicket
        {
            OrderId = t.OrderId,
            Seat = t.Seat,
            PricingType = t.PricingType,
            TicketPrice = t.TicketPrice,
            IsActive = t.IsActive,
            ScreeningId = t.ScreeningId,
            Screening = t.Screening,
            ScreeningDate = t.Screening.ScreeningDate,
            ArchiveDate = today
        }).ToList();

        return archivedTickets;
    }

    /// <summary>
    /// Prepares a list of archived screenings based on the screenings to archive and associated tickets.
    /// </summary>
    /// <param name="screeningsToArchive">The list of screenings to archive.</param>
    /// <param name="ticketsToArchive">The list of tickets to archive.</param>
    /// <returns>A list of archived screenings.</returns>
    private async Task<List<ArchivedScreening>> PrepareArchivedScreenings(List<Screening> screeningsToArchive, List<Ticket> ticketsToArchive)
    {
        List<ArchivedScreening> archivedScreenings = new();

        foreach (Screening screening in screeningsToArchive)
        {
            decimal screeningIncome = ticketsToArchive
                .Where(ticket => ticket.ScreeningId == screening.Id)
                .Sum(ticket => ticket.TicketPrice);

            archivedScreenings.Add(new ArchivedScreening
            {
                Id = screening.Id,
                ScreeningDate = screening.ScreeningDate,
                Subtitles = screening.Subtitles,
                Lector = screening.Lector,
                Dubbing = screening.Dubbing,
                Is3D = screening.Is3D,
                SeatsLeft = screening.SeatsLeft,
                RoomId = screening.RoomId,
                MovieId = screening.MovieId,
                GrossIncome = screeningIncome
            });
        }

        return archivedScreenings;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _timer?.Dispose();
    }
}
