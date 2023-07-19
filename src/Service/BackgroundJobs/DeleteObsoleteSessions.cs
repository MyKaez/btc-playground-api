using Application.Services;
using Domain.Models;
using Infrastructure.Models;
using Infrastructure.Services;
using Service.Extensions;

namespace Service.BackgroundJobs;

public class DeleteObsoleteSessions : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public DeleteObsoleteSessions(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sessionService = _serviceProvider.GetRequiredService<ISessionService>();
        var connectionService = _serviceProvider.GetRequiredService<IConnectionService>();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var connections = connectionService.GetAll();
            var sessions = await sessionService.GetAll(stoppingToken);

            foreach (var session in sessions)
            {
                if (!session.IsDeletable())
                    continue;
                if (HasNoConnection(connections, session))
                    await sessionService.DeleteSession(session.Id, stoppingToken);
                if (IsExpired(session))
                    await sessionService.DeleteSession(session.Id, stoppingToken);
            }

            await Task.Delay(5_000, stoppingToken);
        }
    }

    private static bool HasNoConnection(ICollection<Connection> connections, Session session)
    {
        if (session.Updated.AddSeconds(30) > DateTime.UtcNow)
            return false;

        return connections.All(c => c.SessionId != session.Id);
    }

    private static bool IsExpired(Session session)
    {
        return DateTime.UtcNow > session.ExpiresAt;
    }
}