﻿using Infrastructure.Database;
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
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Session?> Update(Guid id, Action<Session> update, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions.Include(s => s.Interactions)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (session is null)
            return null;

        update(session);

        await _context.SaveChangesAsync(cancellationToken);

        return session;
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        var session = await GetById(id, cancellationToken);
        
        if (session is null)
            return;

        var interactions = _context.Interactions.Where(i => i.SessionId == id).Include(i => i.User).ToArray();
        var users = interactions.Select(i => i.User).Where(i => i != null).Select(i => i!).ToArray();
        var messages = _context.Messages.Where(i => i.SessionId == id).ToArray();
        var connections = _context.Connections.Where(c => c.SessionId == id).ToArray();
        
        _context.Users.RemoveRange(users);
        _context.Interactions.RemoveRange(interactions);
        _context.Messages.RemoveRange(messages);
        _context.Connections.RemoveRange(connections);
        _context.Remove(session);

        await _context.SaveChangesAsync(cancellationToken);
    }
}