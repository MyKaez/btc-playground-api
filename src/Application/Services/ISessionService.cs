using System.Text.Json;
using System.Text.Json.Nodes;
using Domain.Models;

namespace Application.Services;

public interface ISessionService
{
    IEnumerable<Session> GetAll();
    
    Session? GetById(Guid id);

    Task<Session?> CreateService(string name, JsonElement? configuration, CancellationToken cancellationToken);
    
    Task<Session> StartSession(Session session, CancellationToken cancellationToken);
    
    Task<Session> StopSession(Session session, CancellationToken cancellationToken);

    Task<Session> NotifySession(Session session, JsonNode data, CancellationToken cancellationToken);
}