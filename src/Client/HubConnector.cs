using Client.Models;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client;

public class HubConnector
{
    public static async Task<HubConnection> CreateConnection(string baseUrl, Guid sessionId)
    {
        var hubConnectionBuilder = new HubConnectionBuilder()
            .WithUrl(baseUrl + "sessions-hub", o => { o.Transports = HttpTransportType.WebSockets; });
        var connection = hubConnectionBuilder.Build();

        await connection.StartAsync();
        await connection.InvokeCoreAsync("RegisterSession", new object[] { sessionId });

        connection.Closed += async exception =>
        {
            if (exception is not null)
                Console.WriteLine("Error, retrying:" + exception.Message);

            await connection.StartAsync();
            await connection.InvokeCoreAsync("RegisterSession", new object[] { sessionId });
        };
        
        connection.On<Update>(sessionId + ":SessionUpdate", update =>
        {
            Console.WriteLine($"Session update: {update.Status}");
            
            return Task.CompletedTask;
        });
        
        connection.On(sessionId + ":UserUpdates", async () =>
        {
            try
            {
                var http = new HttpClient
                {
                    BaseAddress = new Uri(baseUrl)
                };
                
                await http.GetStringAsync($"v1/sessions/{sessionId}/users");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });

        return connection;
    }
}