using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Polly;

namespace Client;

public class HubConnector
{
    private static readonly HashSet<string> Errors = new();

    private static readonly AsyncPolicy ConnectionPolicy = Policy.Handle<Exception>()
        .WaitAndRetryAsync(5, current => TimeSpan.FromMilliseconds(50 * (current + 1)), (exception, _) =>
        {
            if (Errors.Add(exception.Message))
                Console.WriteLine("Error, retrying: " + exception.Message);
        });

    public static async Task<HubConnection> CreateConnection(string baseUrl, Guid sessionId)
    {
        var hubConnectionBuilder = new HubConnectionBuilder()
            .WithUrl(baseUrl + "sessions-hub", o
                => o.Transports = HttpTransportType.WebSockets);
        var connection = hubConnectionBuilder.Build();
        
        connection.HandshakeTimeout = TimeSpan.FromSeconds(10);
        connection.ServerTimeout = TimeSpan.FromSeconds(10);

        await ConnectionPolicy.ExecuteAsync(async () =>
        {
            await connection.StartAsync();
            await connection.InvokeCoreAsync("RegisterSession", new object[] { sessionId });
        });

        OnSessionClosed(sessionId, connection);
        OnSessoinUpdates(sessionId, connection);
        OnUserUpdatesEvent(baseUrl, sessionId, connection);

        return connection;
    }

    private static void OnSessionClosed(Guid sessionId, HubConnection connection)
    {
        connection.Closed += async exception =>
        {
            if (exception is not null)
                Console.WriteLine("Error, retrying:" + exception.Message);
            
            await ConnectionPolicy.ExecuteAsync(async () =>
            {
                await connection.StartAsync();
                await connection.InvokeCoreAsync("RegisterSession", new object[] { sessionId });
            });
        };
    }

    private static void OnSessoinUpdates(Guid sessionId, HubConnection connection)
    {
        connection.On<JsonElement>(sessionId + ":SessionUpdate", update =>
        {
            Console.WriteLine($"Session update: {update}");

            return Task.CompletedTask;
        });
    }

    private static void OnUserUpdatesEvent(string baseUrl, Guid sessionId, HubConnection connection)
    {
        connection.On(sessionId + ":UserUpdates", async () =>
        {
            await ConnectionPolicy.ExecuteAsync(async () =>
            {
                var http = new HttpClient
                {
                    BaseAddress = new Uri(baseUrl)
                };

                var response = await http.GetAsync($"v1/sessions/{sessionId}/users");

                if (response.StatusCode != HttpStatusCode.OK)
                    Console.WriteLine(response.StatusCode);
            });
        });
    }
}