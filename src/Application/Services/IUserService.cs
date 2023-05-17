using Domain.Models;

namespace Application.Services;

public interface IUserService
{
    Task<User?> GetById(Guid userId, CancellationToken cancellationToken);

    Task<User> Create(Session session, string userName, CancellationToken cancellationToken);

    Task<User?> Update(UserUpdate update, CancellationToken cancellationToken);
    
    Task<User[]> GetBySessionId(Guid sessionId, CancellationToken cancellationToken);
}