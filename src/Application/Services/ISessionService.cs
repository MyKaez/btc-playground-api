using System.Text.Json;
using Domain.Models;

namespace Application.Services;

public interface ISessionService
{
    Session? GetById(Guid id);

    Task<Session?> CreateService(string name, JsonElement? configuration, CancellationToken cancellationToken);
    
    Task<Session> StartSession(Session session, CancellationToken cancellationToken);
    
    Task<Session> StopSession(Session session, CancellationToken cancellationToken);
}