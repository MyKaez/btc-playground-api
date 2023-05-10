using System.Collections.Concurrent;
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
            .WithUrl(testServer.BaseAddress + "v1/sessions", o =>
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

        connection.On<string>(session.Id.ToString(), message => messages.Add(message));

        await connection.StartAsync();

        return new TestHub(connection, messages);
    }
}