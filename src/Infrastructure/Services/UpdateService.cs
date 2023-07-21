using System.Collections.Concurrent;

namespace Infrastructure.Services;

public class UpdateService:IUpdateService
{
    private BlockingCollection<Guid> _sessions = new();

    public void AddUpdate(Guid sessionId)
    {
        if (!_sessions.Contains(sessionId))
            _sessions.TryAdd(sessionId);
    }

    public IEnumerable<Guid> GetUpdates()
    {
        var res = _sessions.ToArray();
        _sessions.Dispose();
        _sessions = new BlockingCollection<Guid>();
        return res;
    }
}