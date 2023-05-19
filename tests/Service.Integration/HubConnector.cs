using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Service.Integration.Models;

namespace Service.Integration;

public class HubConnector
{
    private readonly IHubConnectionBuilder _hubConnectionBuilder;

    public HubConnector(TestServer testServer)
    {
        _hubConnectionBuilder = new HubConnectionBuilder()
            .WithUrl(testServer.BaseAddress + "sessions-hub", o =>
            {
                o.Transports = HttpTransportType.WebSockets;
                o.SkipNegotiation = true;
                o.HttpMessageHandlerFactory = _ => testServer.CreateHandler();
                o.WebSocketFactory = async (context, cancellationToken) =>
                {
                    var wsClient = testServer.CreateWebSocketClient();
                    var url = $"{context.Uri}";

                    return await wsClient.ConnectAsync(new Uri(url), cancellationToken);
                };
            });
    }

    public async Task<TestHub> CreateConnection(
        Session session)
    {
        var connection = _hubConnectionBuilder.Build();
        var messages = new BlockingCollection<string>();

        connection.On<User>(session.Id + ":CreateUser", user => messages.Add(user.Name));
        connection.On<User>(session.Id + ":DeleteUser", user => messages.Add(user.Name));
        connection.On<UserUpdate>(session.Id + ":UserUpdate", user => messages.Add(user.Configuration.ToString()));
        connection.On<SessionUpdate>(session.Id + ":SessionUpdate", update => messages.Add(update.Status));
        connection.On<JsonElement>(session.Id + ":UserMessage", message => messages.Add(message.ToString()));
        
        await connection.StartAsync();

        return new TestHub(connection, messages);
    }
}