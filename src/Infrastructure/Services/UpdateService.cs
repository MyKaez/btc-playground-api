using System.Collections.Concurrent;
using Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public class UpdateService : IUpdateService
{
    private readonly IMemoryCache _memoryCache;
    private BlockingCollection<Guid> _sessions = new();

    public UpdateService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    
    public void AddUpdate(Guid sessionId)
    {
        _memoryCache.Remove(sessionId);
        _memoryCache.Remove(sessionId.UserCacheKey());
        
        if (!_sessions.Contains(sessionId))
            _sessions.TryAdd(sessionId);
    }

    public void RemoveUpdate(Guid sessionId)
    {
        if (!_sessions.Contains(sessionId))
            return;
        var sessions = _sessions.Where(s => s != sessionId).ToArray();
        _sessions.Dispose();
        _sessions = new BlockingCollection<Guid>();
        _sessions.CopyTo(sessions, 0);
    }

    public IEnumerable<Guid> GetUpdates()
    {
        var res = _sessions.ToArray();
        _sessions.Dispose();
        _sessions = new BlockingCollection<Guid>();
        return res;
    }
}