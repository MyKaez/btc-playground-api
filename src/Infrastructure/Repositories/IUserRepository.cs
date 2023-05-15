using Infrastructure.Database;

namespace Infrastructure.Repositories;

public interface IUserRepository
{
    ValueTask<User?> GetById(Guid userId, CancellationToken cancellationToken);

    Task Create(Guid sessionId, User user, CancellationToken cancellationToken);
}