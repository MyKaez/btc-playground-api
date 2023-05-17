using Infrastructure.Database;

namespace Infrastructure.Repositories.Database;

public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _context;

    public UserRepository(DatabaseContext context)
    {
        _context = context;
    }

    public ValueTask<User?> GetById(Guid userId, CancellationToken cancellationToken)
    {
        return _context.FindAsync<User>(new object[] { userId }, cancellationToken);
    }

    public async Task Create(Guid sessionId, User user, CancellationToken cancellationToken)
    {
        var interaction = new Interaction
        {
            Id = user.Id,
            SessionId = sessionId,
            UserId = Guid.NewGuid()
        };

        await _context.AddAsync(interaction, cancellationToken);
        await _context.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> Update(Guid userId, Action<User> update, CancellationToken cancellationToken)
    {
        var session = await GetById(userId, cancellationToken);

        if (session is null)
            return null;

        update(session);
        _context.Update(session);

        await _context.SaveChangesAsync(cancellationToken);

        return session;
    }
}