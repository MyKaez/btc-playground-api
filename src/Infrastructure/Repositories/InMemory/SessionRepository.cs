using System.Collections.Concurrent;
using Infrastructure.Database;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.InMemory;

public class SessionRepository : ISessionRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly BlockingCollection<Session> _sessions;

    public SessionRepository(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _sessions = new BlockingCollection<Session>();
    }

    public ValueTask<Session?> GetById(Guid id, CancellationToken cancellationToken)
    {
        var session = _memoryCache.Get<Session?>(id);

        return ValueTask.FromResult(session);
    }

    public Task<IReadOnlyCollection<Session>> GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<Session>>(_sessions);
    }

    public ValueTask Add(Session entity, CancellationToken cancellationToken)
    {
        _memoryCache.Set(entity.Id, entity);
        _sessions.Add(entity, cancellationToken);

        return ValueTask.CompletedTask;
    }

    public Task<Session?> Update(Guid id, Action<Session> entity, CancellationToken cancellationToken)
    {
        var entry = _memoryCache.Get<Session>(id);

        if (entry is not null)
            entity(entry);

        return Task.FromResult(entry);
    }
}