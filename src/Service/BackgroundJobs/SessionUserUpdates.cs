using Infrastructure.Hubs;
using Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;

namespace Service.BackgroundJobs;

public class SessionUserUpdates : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public SessionUserUpdates(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5_000, stoppingToken);
            
            var hubContext = _serviceProvider.GetRequiredService<IHubContext<SessionHub>>();
            var updateService = _serviceProvider.GetRequiredService<IUpdateService>();
            
            foreach (var sessionId in updateService.GetUpdates())
            {
                updateService.RemoveUpdate(sessionId);
                await hubContext.Clients.All.SendAsync(sessionId + ":UserUpdates", stoppingToken);
            }
        }
    }
}