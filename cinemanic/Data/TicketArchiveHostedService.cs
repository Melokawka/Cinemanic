using cinemanic.Data;

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
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CinemanicDbContext>();
            var ticketArchiveService = new TicketArchiveService(dbContext);
            await ticketArchiveService.ExecuteArchiveAsync(CancellationToken.None);
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
