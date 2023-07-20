using Application.Serialization;
using Application.Services;
using Application.Simulations;
using Domain.Simulations;
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
    private readonly IConnectionService _connectionService;
    private readonly ISessionService _sessionService;
    private readonly ISimulatorFactory _simulatorFactory;
    private readonly ILogger<SessionHub> _logger;

    public SessionHub(ILogger<SessionHub> logger, IConnectionService connectionService, ISessionService sessionService,
        ISimulatorFactory simulatorFactory)
    {
        _logger = logger;
        _connectionService = connectionService;
        _sessionService = sessionService;
        _simulatorFactory = simulatorFactory;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("Initiated connection {Connection}", Context.ConnectionId);

        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connection = await _connectionService.Get(Context.ConnectionId);

        if (connection is null)
        {
            _logger.LogInformation("Disconnected connection {Connection}", Context.ConnectionId);
        }
        else
        {
            await _connectionService.Remove(Context.ConnectionId);
            _logger.LogInformation("Session {SessionID} disconnected from {Connection}",
                connection.SessionId, Context.ConnectionId);

            if (connection.UserId.HasValue)
            {
                var session = await _sessionService.GetById(connection.SessionId, CancellationToken.None);
                var simulationType = session!.Configuration?.FromJsonElement<Simulation>()?.SimulationType ?? "";
                
                await _sessionService.DeleteUser(connection.SessionId, connection.UserId.Value, CancellationToken.None);

                if (simulationType != "")
                {
                    var simulator = _simulatorFactory.Create(simulationType);

                    await simulator.UserDelete(session, connection.UserId.Value, CancellationToken.None);
                }
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    ///     This method is meant to be called by the frontend. In order to interact properly with the api, the session needs to
    ///     be registered here.
    /// </summary>
    public Task RegisterSession(Guid sessionId)
    {
        _connectionService.Add(Context.ConnectionId, sessionId);
        _logger.LogInformation(
            "Session {SessionID} was registered for Connection {Connection}", sessionId, Context.ConnectionId);

        return Task.CompletedTask;
    }

    /// <summary>
    ///     This method is meant to be called by the frontend. In order to interact properly with the api, the session needs to
    ///     be registered here.
    /// </summary>
    public Task RegisterUser(Guid userId)
    {
        _connectionService.Update(Context.ConnectionId, userId);
        _logger.LogInformation(
            "User {UserID} was registered for Connection {Connection}", userId, Context.ConnectionId);

        return Task.CompletedTask;
    }
}