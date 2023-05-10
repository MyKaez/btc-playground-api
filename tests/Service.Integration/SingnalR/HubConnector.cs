using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Service.Integration.Models;

namespace Service.Integration.SingnalR;

public class HubConnector
{
    private readonly IHubConnectionBuilder _hubConnectionBuilder;
    private readonly BlockingCollection<string> _messages = new();

    public HubConnector(TestServer testServer)
    {
        _hubConnectionBuilder = new HubConnectionBuilder()
            .WithUrl(testServer.BaseAddress + "v1/sessions", o =>
            {
                o.Transports = HttpTransportType.WebSockets;
                o.AccessTokenProvider = async () => await Task.FromResult("");
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

    public IEnumerable<string> Messages => _messages;

    public async Task<HubConnection> CreateConnection(Session session)
    {
        var connection = _hubConnectionBuilder.Build();

        connection.On<string>(session.Id.ToString(), message => _messages.Add(message));

        await connection.StartAsync();

        return connection;
    }
}