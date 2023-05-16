using System.Text.Json;
using Domain.Models;

namespace Application.Services;

public interface ISessionService
{
    Task<Session?> GetById(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<Session>> GetAll(CancellationToken cancellationToken);

    Task<Session?> CreateSession(string name, JsonElement? configuration, CancellationToken cancellationToken);

    Task<Session?> UpdateSession(SessionUpdate update, CancellationToken cancellationToken);
    
    Task<Session?> NotifySession(Guid sessionId, JsonElement data, CancellationToken cancellationToken);
}