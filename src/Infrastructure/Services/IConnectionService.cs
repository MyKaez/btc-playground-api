using Infrastructure.Models;

namespace Infrastructure.Services;

public interface IConnectionService
{
    Task<ICollection<Connection>> GetAll();

    Task Add(string connectionId, Guid sessionId);
    
    Task Update(string contextConnectionId, Guid userId);

    Task Remove(string connectionId);
    
    Task<Connection?> Get(string connectionId);
}