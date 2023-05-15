using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Hubs;

// https://code-maze.com/netcore-signalr-angular-realtime-charts/
// https://guidnew.com/en/blog/signalr-modules-with-a-shared-connection-using-a-csharp-source-generator/

// https://code-maze.com/how-to-send-client-specific-messages-using-signalr/
// https://learn.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/working-with-groups
public class SessionHub : Hub
{
    private readonly ILogger<SessionHub> _logger;

    public SessionHub(ILogger<SessionHub> logger)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("{Name} connected", Context.ConnectionId);

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("{Name} disconnected", Context.ConnectionId);
        
        return base.OnDisconnectedAsync(exception);
    }
}