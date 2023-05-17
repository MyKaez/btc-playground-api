using Infrastructure.Database;

namespace Infrastructure.Repositories;

public interface IUserRepository
{
    ValueTask<User?> GetById(Guid userId, CancellationToken cancellationToken);

    Task Create(Guid sessionId, User user, CancellationToken cancellationToken);
    
    Task<User?> Update(Guid userId, Action<User> update, CancellationToken cancellationToken);
    
    Task<User[]> GetBySessionId(Guid sessionId, CancellationToken cancellationToken);
}