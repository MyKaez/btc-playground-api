using System.Text.Json;
using Domain.Models;

namespace Application.Services;

public interface ISessionService
{
    Session? GetById(Guid id);

    Session? CreateService(string name, JsonElement? configuration);
    
    Session StartSession(Session session);
    
    Session StopSession(Session session);
}