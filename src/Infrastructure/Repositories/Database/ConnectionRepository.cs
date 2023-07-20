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
        throw new NotImplementedException();
    }

    public void Update(string connectionId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public void Remove(string connectionId)
    {
        throw new NotImplementedException();
    }

    public Connection? Get(string connectionId)
    {
        throw new NotImplementedException();
    }
}