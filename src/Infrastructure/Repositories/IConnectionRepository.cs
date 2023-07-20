using Infrastructure.Database;

namespace Infrastructure.Repositories;

public interface IConnectionRepository
{
    ICollection<Connection> GetAll();

    void Add(string connectionId, Guid sessionId);

    void Update(string connectionId, Guid userId);

    void Remove(string connectionId);

    Connection? Get(string connectionId);
}