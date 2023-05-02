using Domain.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace Service.Hubs;

public class SessionHub : Hub
{
    private readonly IMemoryCache _memoryCache;

    public SessionHub(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task SendMessage(Guid sessionId)
    {
        var session =_memoryCache.Get<Session>(sessionId);

        if (session is null)
            return;

        await Clients.Client(sessionId.ToString()).SendAsync("", "");
    }
}