using System.Text.Json;
using Application.Models;
using Domain.Models;

namespace Application.Simulations;

public interface ISimulator
{
    Task<Result<JsonElement>> SessionPrepare(Session session, JsonElement config, CancellationToken cancellationToken);

    Task<Result<JsonElement>> SessionStart(Session session, JsonElement config, CancellationToken cancellationToken);

    Task<Result<JsonElement>> SessionStop(Session session, JsonElement config, CancellationToken cancellationToken);

    Task<Result<JsonElement>> SessionReset(Session session, JsonElement config, CancellationToken cancellationToken);

    Task<Result<JsonElement>> SessionUpdate(Session session, JsonElement config, CancellationToken cancellationToken);
    
    Task<Result<JsonElement>> UserNotReady(Session session, JsonElement config, CancellationToken cancellationToken);

    Task<Result<JsonElement>> UserReady(
        Session session, User user, JsonElement config, CancellationToken cancellationToken);

    Task<Result<JsonElement>> UserDone(Session session, JsonElement config, CancellationToken cancellationToken);

    Task UserDelete(Session session, Guid userId, CancellationToken cancellationToken);
}