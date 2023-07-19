using Application.Services;
using Domain.Models;
using Infrastructure.Models;
using Infrastructure.Services;
using Service.Extensions;

namespace Service.BackgroundJobs;

public class DeleteObsoleteSessions : BackgroundService
{
    private readonly ISessionService _sessionService;
    private readonly IConnectionService _connectionService;

    public DeleteObsoleteSessions(ISessionService sessionService, IConnectionService connectionService)
    {
        _sessionService = sessionService;
        _connectionService = connectionService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var connections = _connectionService.GetAll();
            var sessions = await _sessionService.GetAll(stoppingToken);

            foreach (var session in sessions)
            {
                if (!session.IsDeletable())
                    continue;
                if (HasNoConnection(connections, session))
                    await _sessionService.DeleteSession(session.Id, stoppingToken);
                if (IsExpired(session))
                    await _sessionService.DeleteSession(session.Id, stoppingToken);
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