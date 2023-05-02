using Application.Services;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IMemoryCache _memoryCache;

    public UserService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public User Create(Session session, string userName)
    {
        var user = new User
        {
            Id = Guid.NewGuid(), Name = userName
        };
        var options = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        };
        var newSession = session with { ExpiresIn = options.SlidingExpiration.Value};
        
        _memoryCache.Set(session.Id, newSession.Add(user), options);

        return user;
    }
}