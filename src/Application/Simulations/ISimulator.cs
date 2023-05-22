using System.Text.Json;
using Application.Models;
using Domain.Models;

namespace Application.Simulations;

public interface ISimulator
{
    Task<RequestResult<JsonElement>?> SessionPrepare(Session session, JsonElement config,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<RequestResult<JsonElement>?>(null);
    }

    Task<RequestResult<JsonElement>?> SessionStart(Session session, JsonElement config,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<RequestResult<JsonElement>?>(null);
    }

    Task<RequestResult<JsonElement>?> SessionStop(Session session, JsonElement config,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<RequestResult<JsonElement>?>(null);
    }

    Task<RequestResult<JsonElement>?> SessionReset(Session session, JsonElement config,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<RequestResult<JsonElement>?>(null);
    }

    Task<RequestResult<JsonElement>?> UserNotReady(Session session, JsonElement config,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<RequestResult<JsonElement>?>(null);
    }

    Task<RequestResult<JsonElement>?> UserReady(Session session, User user, JsonElement config,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<RequestResult<JsonElement>?>(null);
    }

    Task<RequestResult<JsonElement>?> UserDone(Session session, JsonElement config, CancellationToken cancellationToken)
    {
        return Task.FromResult<RequestResult<JsonElement>?>(null);
    }

    Task UserDelete(Session session, Guid userId, CancellationToken cancellationToken);
}