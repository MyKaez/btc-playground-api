using Application.Serialization;
using Application.Services;
using Application.Simulations;
using Domain.Simulations;
using Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Hubs;

// https://code-maze.com/netcore-signalr-angular-realtime-charts/
// https://guidnew.com/en/blog/signalr-modules-with-a-shared-connection-using-a-csharp-source-generator/

// https://code-maze.com/how-to-send-client-specific-messages-using-signalr/
// https://learn.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/working-with-groups
public class SessionHub : Hub
{
    private readonly ILogger<SessionHub> _logger;
    private readonly IServiceProvider _serviceProvider;

    public SessionHub(ILogger<SessionHub> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("Initiated connection {Connection}", Context.ConnectionId);

        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionService = _serviceProvider.GetRequiredService<IConnectionService>();
        var connection = await connectionService.Get(Context.ConnectionId);

        if (connection is null)
        {
            _logger.LogInformation("Disconnected connection {Connection}", Context.ConnectionId);
        }
        else
        {
            await connectionService.Remove(Context.ConnectionId);
            
            _logger.LogInformation("Session {SessionID} disconnected from {Connection}",
                connection.SessionId, Context.ConnectionId);

            if (connection.UserId.HasValue)
            {
                var sessionService = _serviceProvider.GetRequiredService<ISessionService>();
                var session = await sessionService.GetById(connection.SessionId, CancellationToken.None);
                var simulationType = session!.Configuration?.FromJsonElement<Simulation>()?.SimulationType ?? "";
                
                await sessionService.DeleteUser(connection.SessionId, connection.UserId.Value, CancellationToken.None);

                if (simulationType != "")
                {
                    var simulatorFactory = _serviceProvider.GetRequiredService<ISimulatorFactory>();
                    var simulator = simulatorFactory.Create(simulationType);

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
    public async Task RegisterSession(Guid sessionId)
    {
        var connectionService = _serviceProvider.GetRequiredService<IConnectionService>();
        
        await connectionService.Add(Context.ConnectionId, sessionId);
        
        _logger.LogInformation(
            "Session {SessionID} was registered for Connection {Connection}", sessionId, Context.ConnectionId);
    }

    /// <summary>
    ///     This method is meant to be called by the frontend. In order to interact properly with the api, the session needs to
    ///     be registered here.
    /// </summary>
    public async  Task RegisterUser(Guid userId)
    {
        var connectionService = _serviceProvider.GetRequiredService<IConnectionService>();
        
        await connectionService.Update(Context.ConnectionId, userId);
        
        _logger.LogInformation(
            "User {UserID} was registered for Connection {Connection}", userId, Context.ConnectionId);
    }
}