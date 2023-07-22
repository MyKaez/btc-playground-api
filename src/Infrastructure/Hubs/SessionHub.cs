using Application.Serialization;
using Application.Services;
using Application.Simulations;
using Domain.Simulations;
using Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Infrastructure.Hubs;

// https://code-maze.com/netcore-signalr-angular-realtime-charts/
// https://guidnew.com/en/blog/signalr-modules-with-a-shared-connection-using-a-csharp-source-generator/

// https://code-maze.com/how-to-send-client-specific-messages-using-signalr/
// https://learn.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/working-with-groups
public class SessionHub : Hub
{
    private readonly ILogger<SessionHub> _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly AsyncPolicy _retryPolicy = Policy
        .Handle<SqlException>(e =>
            e.Message.Contains("deadlocked on lock resources with another process"))
        .WaitAndRetryAsync(5, current => TimeSpan.FromMilliseconds(50 * (current + 1)));

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
        if (exception is not null)
            _logger.LogError(exception, "Erroneously disconnected connection {Connection}", Context.ConnectionId);

        await _retryPolicy.ExecuteAsync(async () =>
        {
            var connectionService = _serviceProvider.GetRequiredService<IConnectionService>();
            var connection = await connectionService.Get(Context.ConnectionId);

            if (connection is null)
            {
                _logger.LogInformation("Disconnected connection {Connection}", Context.ConnectionId);
            }
            else
            {
                _logger.LogInformation("Session {SessionID} disconnected from {Connection}",
                    connection.SessionId, Context.ConnectionId);

                if (connection.UserId.HasValue)
                    await DisconnectUser(connection.SessionId, connection.UserId.Value);
                else
                    await connectionService.Remove(Context.ConnectionId);
            }
        });

        await base.OnDisconnectedAsync(exception);
    }

    private async Task DisconnectUser(Guid sessionId, Guid userId)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            var sessionService = _serviceProvider.GetRequiredService<ISessionService>();
            var session = await sessionService.GetById(sessionId, CancellationToken.None);
            var simulationType = session!.Configuration?.FromJsonElement<Simulation>()?.SimulationType ?? "";

            await sessionService.DeleteUser(sessionId, userId,
                CancellationToken.None);

            if (simulationType != "")
            {
                var simulatorFactory = _serviceProvider.GetRequiredService<ISimulatorFactory>();
                var simulator = simulatorFactory.Create(simulationType);

                await simulator.UserDelete(session, userId, CancellationToken.None);
            }
        });
    }

    /// <summary>
    ///     This method is meant to be called by the frontend. In order to interact properly with the api, the session needs to
    ///     be registered here.
    /// </summary>
    public async Task RegisterSession(Guid sessionId)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            var connectionService = _serviceProvider.GetRequiredService<IConnectionService>();

            await connectionService.Add(Context.ConnectionId, sessionId);

            _logger.LogInformation(
                "Session {SessionID} was registered for Connection {Connection}", sessionId, Context.ConnectionId);
        });
    }

    /// <summary>
    ///     This method is meant to be called by the frontend. In order to interact properly with the api, the session needs to
    ///     be registered here.
    /// </summary>
    public async Task RegisterUser(Guid userId)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            var connectionService = _serviceProvider.GetRequiredService<IConnectionService>();

            await connectionService.Update(Context.ConnectionId, userId);

            _logger.LogInformation(
                "User {UserID} was registered for Connection {Connection}", userId, Context.ConnectionId);
        });
    }

    public Task Alive(Guid? userId)
    {
        if (userId.HasValue)
            _logger.LogInformation("User {UserID} is alive", userId);
        else
            _logger.LogInformation("Host is alive");
        
        return Task.CompletedTask;
    }
}