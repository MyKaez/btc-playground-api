using Infrastructure.Database;

namespace Infrastructure.Repositories.Database;

public class ConnectionRepository : IConnectionRepository
{
    private readonly DatabaseContext _context;

    public ConnectionRepository(DatabaseContext context)
    {
        _context = context;
    }

    public Task<ICollection<Connection>> GetAll()
    {
        return Task.FromResult<ICollection<Connection>>(_context.Connections.ToList());
    }

    public async Task Add(string connectionId, Guid sessionId)
    {
        var connection = new Connection { Id = connectionId, SessionId = sessionId };

        await _context.AddAsync(connection);
        await _context.SaveChangesAsync();
    }

    public async Task Update(string connectionId, Guid userId)
    {
        var connection = await _context.Connections.FindAsync(connectionId);
        var user = await _context.Users.FindAsync(userId);

        connection!.UserId = userId;
        user!.ConnectionId = connectionId;

        await _context.SaveChangesAsync();
    }

    public async Task Remove(string connectionId)
    {
        var con = await _context.Connections.FindAsync(connectionId);

        if (con is null)
            return;
        
        _context.Remove(con);
        await _context.SaveChangesAsync();
    }

    public async Task<Connection?> Get(string connectionId)
    {
        return await _context.Connections.FindAsync(connectionId);
    }
}