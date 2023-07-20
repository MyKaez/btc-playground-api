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

    public Task<ICollection<Connection>> GetAll()
    {
        var connections = _memoryCache.GetOrCreate<List<Connection>>(
            "Connections",
            _ => new List<Connection>()
        );

        return Task.FromResult<ICollection<Connection>>(connections!);
    }

    public async Task Add(string connectionId, Guid sessionId)
    {
        var connections = await GetAll();
        var con = new Connection { Id = connectionId, SessionId = sessionId };

        connections.Add(con);
    }

    public async Task Update(string connectionId, Guid userId)
    {
        var connection = await Get(connectionId);

        if (connection is not null)
            connection.UserId = userId;
    }

    public async Task Remove(string connectionId)
    {
        var connections = await GetAll();
        var con = connections.First(c => c.Id == connectionId);

        connections.Remove(con);
    }

    public async Task<Connection?> Get(string connectionId)
    {
        var connections = await GetAll();
        var con = connections.FirstOrDefault(c => c.Id == connectionId);

        return con;
    }
}