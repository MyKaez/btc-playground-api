using Infrastructure.Database;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.InMemory;

public class ConnectionRepository : IConnectionRepository
{
    private readonly IMemoryCache _memoryCache;

    public ConnectionRepository(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public ICollection<Connection> GetAll()
    {
        var connections = _memoryCache.GetOrCreate<List<Connection>>(
            "Connections",
            _ => new List<Connection>()
        );

        return connections!;
    }

    public void Add(string connectionId, Guid sessionId)
    {
        var connections = GetAll();
        var con = new Connection { Id = connectionId, SessionId = sessionId };

        connections.Add(con);
    }

    public void Update(string connectionId, Guid userId)
    {
        var connection = Get(connectionId);

        if (connection is not null)
            connection.UserId = userId;
    }

    public void Remove(string connectionId)
    {
        var connections = GetAll();
        var con = connections.First(c => c.Id == connectionId);

        connections.Remove(con);
    }

    public Connection? Get(string connectionId)
    {
        var connections = GetAll();
        var con = connections.FirstOrDefault(c => c.Id == connectionId);

        return con;
    }
}