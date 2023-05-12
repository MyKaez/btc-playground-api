using System.Text.Json.Nodes;
using Application.Services;
using Domain.Models;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IHubContext<SessionHub> _hubContext;

    public UserService(IMemoryCache memoryCache, IHubContext<SessionHub> hubContext)
    {
        _memoryCache = memoryCache;
        _hubContext = hubContext;
    }

    public User? GetById(Session session, Guid userId)
    {
        return session.Users.FirstOrDefault(user => user.Id == userId);
    }

    public async Task<User> Create(Session session, string userName, CancellationToken cancellationToken)
    {
        var user = new User { Id = Guid.NewGuid(), Name = userName };
        var options = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };
        var newSession = session with { ExpiresIn = options.SlidingExpiration.Value };

        await _hubContext.Clients.All.SendAsync(session.Id + ":CreateUser", user, cancellationToken);

        _memoryCache.Set(session.Id, newSession.Add(user), options);

        return user;
    }

    public async Task Execute(Session session, User user, JsonNode data, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.SendAsync(session.Id.ToString(), data, cancellationToken);
    }
}