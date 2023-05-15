using System.Text.Json;
using System.Text.Json.Nodes;
using Domain.Models;

namespace Application.Services;

public interface ISessionService
{
    Task<Session?> GetById(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<Session>> GetAll(CancellationToken cancellationToken);

    Task<Session?> CreateSession(string name, JsonElement? configuration, CancellationToken cancellationToken);

    Task<Session?> StartSession(Guid sessionId, CancellationToken cancellationToken);

    Task<Session?> StopSession(Guid sessionId, CancellationToken cancellationToken);

    Task<Session?> NotifySession(Guid sessionId, JsonNode data, CancellationToken cancellationToken);
}