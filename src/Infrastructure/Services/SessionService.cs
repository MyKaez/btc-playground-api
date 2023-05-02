using System.Text.Json;
using Application.Services;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public class SessionService : ISessionService
{
    private readonly IMemoryCache _memoryCache;

    public SessionService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Session? GetById(Guid id)
    {
        if (!_memoryCache.TryGetValue<Session>(id, out var session) || session is null)
            return null;

        var options = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        };

        session = session with { ExpiresIn = options.SlidingExpiration.Value };

        _memoryCache.Set(id, session, options);

        return session;
    }

    public Session? CreateService(string name, JsonElement? configuration)
    {
        var session = new Session
        {
            Id = Guid.NewGuid(),
            ControlId = Guid.NewGuid(),
            Name = name,
            Configuration = configuration
        };

        return _memoryCache.GetOrCreate(session.Id, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(5);

            return session with { ExpiresIn = entry.SlidingExpiration.Value };
        });
    }
}