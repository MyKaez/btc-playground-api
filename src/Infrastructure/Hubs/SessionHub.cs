using Domain.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Hubs;

// https://code-maze.com/netcore-signalr-angular-realtime-charts/
// https://guidnew.com/en/blog/signalr-modules-with-a-shared-connection-using-a-csharp-source-generator/
public class SessionHub : Hub
{
    private readonly IMemoryCache _memoryCache;

    public SessionHub(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task SendMessage(Guid sessionId, string message)
    {
        var session =_memoryCache.Get<Session>(sessionId);

        if (session is null)
            return;

        await Clients.All.SendAsync(sessionId.ToString(), message);
    }
}