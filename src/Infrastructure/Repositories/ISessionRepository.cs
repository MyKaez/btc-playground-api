using Infrastructure.Database;

namespace Infrastructure.Repositories;

public interface ISessionRepository
{
    ValueTask<Session?> GetById(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Session>> GetAll(CancellationToken cancellationToken);

    ValueTask Add(Session entity, CancellationToken cancellationToken);

    Task<Session?> Update(Guid id, Action<Session> update, CancellationToken cancellationToken);
    
    Task Delete(Guid id, CancellationToken cancellationToken);
}