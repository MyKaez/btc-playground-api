using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using Client.Models;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Service.Models.Requests;


const string baseUrl = "https://api.btcis.me/";
    //"https://localhost:5001/";

var http = new HttpClient
{
    BaseAddress = new Uri(baseUrl + "v1/")
};

var create = new SessionRequest
{
    Name = "Kenny",
    Configuration = null
};
var res = await http.PostAsJsonAsync("sessions", create);
var opt = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
var session = await res.Content.ReadFromJsonAsync<ControlObject>(opt);
var hubConnectionBuilder = new HubConnectionBuilder()
    .WithUrl(baseUrl + "sessions-hub", o => { o.Transports = HttpTransportType.WebSockets; });
var connection = hubConnectionBuilder.Build();

await connection.StartAsync();
await connection.InvokeCoreAsync("RegisterSession", new object[] { session.Id });

var connections = new BlockingCollection<HubConnection>();

connections.Add(connection);

var stopwatch = Stopwatch.StartNew();
var tasks = Enumerable.Range(0, 300).Select(_ => CreateTask(connections, session)).ToArray();

await Parallel.ForEachAsync(tasks, async (innerTask, _) => await innerTask);

Console.WriteLine($"Created {connections.Count} connections in {stopwatch.Elapsed}");
stopwatch.Stop();

foreach (var con in connections)
{
    await con.DisposeAsync();
}

async Task CreateTask(BlockingCollection<HubConnection> blockingCollection, ControlObject controlObject)
{
    try
    {
        await Task.Delay(Random.Shared.Next(0, 30_000));

        var userConnectionBuilder = new HubConnectionBuilder()
            .WithUrl(baseUrl + "sessions-hub", o => { o.Transports = HttpTransportType.WebSockets; });
        var userConnection = userConnectionBuilder.Build();
        await userConnection.StartAsync();
        await userConnection.InvokeCoreAsync("RegisterSession", new object[] { controlObject.Id });

        var userHttp = new HttpClient
        {
            BaseAddress = new Uri(baseUrl + "v1/sessions/")
        };
        var user = new UserRequest { Name = "Kenny" + Guid.NewGuid().ToString().Split('-')[0] };
        var userRes = await userHttp.PostAsJsonAsync($"{controlObject.Id}/users", user);
        var userX = await userRes.Content.ReadFromJsonAsync<ControlObject>();

        await userConnection.InvokeCoreAsync("RegisterUser", new object[] { userX.Id });

        blockingCollection.Add(userConnection);
    }
    catch (Exception e)
    {
        Console.WriteLine("Error: " + e.Message);
    }
}