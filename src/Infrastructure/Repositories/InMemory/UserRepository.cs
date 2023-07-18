using Infrastructure.Database;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.InMemory;

public class UserRepository : IUserRepository
{
    private readonly IMemoryCache _memoryCache;

    public UserRepository(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public ValueTask<User?> GetById(Guid userId, CancellationToken cancellationToken)
    {
        var user = _memoryCache.Get<User>(userId);

        return ValueTask.FromResult(user);
    }

    public Task Create(Guid sessionId, User user, CancellationToken cancellationToken)
    {
        var session = _memoryCache.Get<Session>(sessionId)!;
        var interaction = new Interaction
        {
            Session = session,
            User = user
        };

        user.Interactions.Add(interaction);
        session.Interactions.Add(interaction);

        _memoryCache.Set(user.Id, user);

        return Task.CompletedTask;
    }

    public async Task<User?> Update(Guid userId, Action<User> update, CancellationToken cancellationToken)
    {
        var user = await GetById(userId, cancellationToken);

        if (user is not null)
            update(user);

        return user;
    }

    public Task<User[]> GetBySessionId(Guid sessionId, CancellationToken cancellationToken)
    {
        var session = _memoryCache.Get<Session>(sessionId)!;
        var users = session.Interactions.Where(i => !i.IsDeleted).Select(i => i.User!).ToArray();

        return Task.FromResult(users);
    }
}