using System.Text.Json;
using System.Text.Json.Nodes;
using Application.Services;
using Domain.Models;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public class SessionService : ISessionService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IHubContext<SessionHub> _hubContext;

    public SessionService(IMemoryCache memoryCache, IHubContext<SessionHub> hubContext)
    {
        _memoryCache = memoryCache;
        _hubContext = hubContext;
    }

    public IEnumerable<Session> GetAll()
    {
        return Array.Empty<Session>();
    }

    public Session? GetById(Guid id)
    {
        if (!_memoryCache.TryGetValue<Session>(id, out var session) || session is null)
            return null;

        var options = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };

        session = session with { ExpiresIn = options.SlidingExpiration.Value };

        _memoryCache.Set(id, session, options);

        return session;
    }

    public async Task<Session?> CreateService(
        string name, JsonElement? configuration, CancellationToken cancellationToken)
    {
        var session = new Session
        {
            Id = Guid.NewGuid(),
            ControlId = Guid.NewGuid(),
            Name = name,
            Configuration = configuration
        };

        await _hubContext.Clients.All.SendAsync(
            session.Id.ToString(), new { session.Id, session.Status }, cancellationToken
        );

        return _memoryCache.GetOrCreate(session.Id, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(5);

            return session with { ExpiresIn = entry.SlidingExpiration.Value };
        });
    }

    public async Task<Session> StartSession(Session session, CancellationToken cancellationToken)
    {
        var options = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };

        session = session with
        {
            Status = SessionStatus.Started,
            ExpiresIn = options.SlidingExpiration.Value
        };

        await _hubContext.Clients.All.SendAsync(
            session.Id.ToString(), new { session.Id, session.Status }, cancellationToken);

        _memoryCache.Set(session.Id, session, options);

        return session;
    }

    public async Task<Session> StopSession(Session session, CancellationToken cancellationToken)
    {
        var options = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };

        session = session with
        {
            Status = SessionStatus.Stopped,
            ExpiresIn = options.SlidingExpiration.Value
        };

        await _hubContext.Clients.All.SendAsync(
            session.Id.ToString(), new { session.Id, session.Status }, cancellationToken);

        _memoryCache.Set(session.Id, session, options);

        return session;
    }

    public async Task<Session> NotifySession(Session session, JsonNode data, CancellationToken cancellationToken)
    {
        var options = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };

        session = session with
        {
            ExpiresIn = options.SlidingExpiration.Value
        };

        await _hubContext.Clients.All.SendAsync(session.Id.ToString(), data, cancellationToken);

        _memoryCache.Set(session.Id, session, options);

        return session;
    }
}