using Infrastructure.Database;

namespace Infrastructure.Repositories.Database;

public class ConnectionRepository : IConnectionRepository
{
    private readonly DatabaseContext _context;

    public ConnectionRepository(DatabaseContext context)
    {
        _context = context;
    }

    public ICollection<Connection> GetAll()
    {
        return _context.Connections.ToList();
    }

    public void Add(string connectionId, Guid sessionId)
    {
        var connection = new Connection{Id = connectionId, SessionId = sessionId};
        
        _context.Add(connection);
        _context.SaveChanges();
    }

    public void Update(string connectionId, Guid userId)
    {
        var connection = _context.Connections.First(c => c.Id == connectionId);
        var user = _context.Users.First(u => u.Id == userId);
        
        connection.UserId = userId;
        user.ConnectionId = connectionId;
        
        _context.SaveChanges();
    }

    public void Remove(string connectionId)
    {
        var con = _context.Connections.First(c => c.Id == connectionId);

        _context.Remove(con);
        _context.SaveChanges();
    }

    public Connection? Get(string connectionId)
    {
        return _context.Connections.Find(connectionId);
    }
}