using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Database;

public class SessionRepository : ISessionRepository
{
    private readonly DatabaseContext _context;

    public SessionRepository(DatabaseContext context)
    {
        _context = context;
    }

    public ValueTask<Session?> GetById(Guid id, CancellationToken cancellationToken)
    {
        return _context.FindAsync<Session>(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Session>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Sessions.ToArrayAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask Add(Session entity, CancellationToken cancellationToken)
    {
        await _context.Sessions.AddAsync(entity, cancellationToken);
    }

    public async Task<Session?> Update(Guid id, Action<Session> update, CancellationToken cancellationToken)
    {
        var session = await GetById(id, cancellationToken);

        if (session is null)
            return null;

        update(session);
        _context.Update(session);

        await _context.SaveChangesAsync(cancellationToken);

        return session;
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        var session = await GetById(id, cancellationToken);
        
        if (session is null)
            return;

        _context.RemoveRange(session.Messages);
        _context.RemoveRange(session.Interactions);
        _context.RemoveRange(session.Interactions.Select(i => i.User).Where(u => u != null).Select(u => u!));
        _context.Remove(session);

        await _context.SaveChangesAsync(cancellationToken);
    }
}