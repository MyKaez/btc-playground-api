using Client.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client;

public class HubConnector
{
    private readonly Session _session;

    public HubConnector(Session session)
    {
        _session = session;
    }

    public async Task<HubConnection> CreateConnection()
    {
        var connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:5001/v1/sessions")
            .Build();

        connection.On<string>(
            _session.Id.ToString(),
            message => Console.WriteLine($"Received message: {message}"));

        await connection.StartAsync();

        Console.WriteLine("SignalR connection started.");

        return connection;
    }
}