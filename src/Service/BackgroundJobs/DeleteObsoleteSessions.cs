using Application.Services;
using Domain.Models;
using Infrastructure.Models;
using Infrastructure.Services;

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
            var connections = await connectionService.GetAll();
            var sessions = await sessionService.GetAll(stoppingToken);

            foreach (var session in sessions)
            {
                if (HasNoConnection(connections, session))
                    await DeleteSession(stoppingToken, session);
                else if (IsExpired(session))
                    await DeleteSession(stoppingToken, session);
                // todo implement "all users are deleted" > User.IsDeleted
            }

            await Task.Delay(30_000, stoppingToken);
        }
    }

    private async Task DeleteSession(CancellationToken stoppingToken, Session session)
    {
        var sessionService = _serviceProvider.GetRequiredService<ISessionService>();

        await sessionService.DeleteSession(session.Id, stoppingToken);
    }

    private static bool HasNoConnection(IEnumerable<Connection> connections, Session session)
    {
        if (session.Updated.AddSeconds(60) > DateTime.UtcNow)
            return false;

        return connections.All(c => c.SessionId != session.Id);
    }

    private static bool IsExpired(Session session)
    {
        return DateTime.UtcNow > session.ExpiresAt;
    }
}