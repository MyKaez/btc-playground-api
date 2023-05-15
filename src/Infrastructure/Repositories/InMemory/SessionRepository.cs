﻿using System.Collections.Concurrent;
using Infrastructure.Database;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.InMemory;

public class SessionRepository : ISessionRepository
{
    private static readonly BlockingCollection<Session> Sessions = new();
    
    private readonly IMemoryCache _memoryCache;

    public SessionRepository(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public ValueTask<Session?> GetById(Guid id, CancellationToken cancellationToken)
    {
        var session = _memoryCache.Get<Session?>(id);

        return ValueTask.FromResult(session);
    }

    public Task<IReadOnlyCollection<Session>> GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<Session>>(Sessions);
    }

    public ValueTask Add(Session entity, CancellationToken cancellationToken)
    {
        _memoryCache.Set(entity.Id, entity);
        Sessions.Add(entity, cancellationToken);

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