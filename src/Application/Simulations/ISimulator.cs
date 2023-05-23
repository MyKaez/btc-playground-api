using System.Text.Json;
using Application.Models;
using Domain.Models;

namespace Application.Simulations;

public interface ISimulator
{
    Task<JsonElement?> SessionPrepare(Session session, JsonElement config, CancellationToken cancellationToken)
    {
        return Task.FromResult<JsonElement?>(null);
    }

    Task<JsonElement?> SessionStart(Session session, JsonElement config, CancellationToken cancellationToken)
    {
        return Task.FromResult<JsonElement?>(null);
    }

    Task<JsonElement?> SessionStop(Session session, JsonElement config, CancellationToken cancellationToken)
    {
        return Task.FromResult<JsonElement?>(null);
    }

    Task<JsonElement?> SessionReset(Session session, JsonElement config, CancellationToken cancellationToken)
    {
        return Task.FromResult<JsonElement?>(null);
    }

    Task<JsonElement?> UserNotReady(Session session, JsonElement config, CancellationToken cancellationToken)
    {
        return Task.FromResult<JsonElement?>(null);
    }

    Task<JsonElement?> UserReady(Session session, User user, JsonElement config, CancellationToken cancellationToken)
    {
        return Task.FromResult<JsonElement?>(null);
    }

    Task<JsonElement> UserDone(Session session, JsonElement config, CancellationToken cancellationToken);

    Task UserDelete(Session session, Guid userId, CancellationToken cancellationToken);
}