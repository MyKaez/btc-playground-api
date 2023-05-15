using Infrastructure.Services;
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
    private readonly IConnectionService _connectionService;

    public SessionHub(ILogger<SessionHub> logger, IConnectionService connectionService)
    {
        _logger = logger;
        _connectionService = connectionService;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("Initiated connection {Connection}", Context.ConnectionId);

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var connection = _connectionService.Get(Context.ConnectionId);
        
        _connectionService.Remove(Context.ConnectionId);
        _logger.LogInformation("Session {SessionID} disconnected from {Connection}", 
            connection.SessionId, Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }

    public Task RegisterSession(Guid sessionId)
    {
        _connectionService.Add(Context.ConnectionId, sessionId);
        _logger.LogInformation(
            "Session {SessionID} was registered for Connection {Connection}", sessionId, Context.ConnectionId);

        return Task.CompletedTask;
    }
}