using Infrastructure.Models;

namespace Infrastructure.Services;

public interface IConnectionService
{
    ICollection<Connection> GetAll();

    void Add(string connectionId, Guid sessionId);
    
    void Update(string contextConnectionId, Guid userId);

    void Remove(string connectionId);
    
    Connection? Get(string connectionId);
}