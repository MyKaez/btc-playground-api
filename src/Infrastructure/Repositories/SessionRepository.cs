using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly DatabaseContext _databaseContext;

    public SessionRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public ValueTask<Session?> GetById(Guid id, CancellationToken cancellationToken)
    {
        return _databaseContext.FindAsync<Session>(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Session>> GetAll(CancellationToken cancellationToken)
    {
        return await _databaseContext.Sessions.ToArrayAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask Add(Session entity, CancellationToken cancellationToken)
    {
        await _databaseContext.Sessions.AddAsync(entity, cancellationToken);
    }

    public async Task<Session?> Update(Guid id, Action<Session> entity, CancellationToken cancellationToken)
    {
        var session = await GetById(id, cancellationToken);

        if (session is null)
            return null;

        entity(session);
        _databaseContext.Update(session);

        await _databaseContext.SaveChangesAsync(cancellationToken);

        return session;
    }
}