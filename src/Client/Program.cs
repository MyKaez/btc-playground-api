using System.Collections.Concurrent;
using System.Diagnostics;
using Client;
using Microsoft.AspNetCore.SignalR.Client;

const string baseUrl = "https://api.btcis.me/";
//"https://localhost:5001/";

var session = await JobService.CreateSession(baseUrl);
var connection = await HubConnector.CreateConnection(baseUrl, session.Id);
var connections = new BlockingCollection<HubConnection>();

connections.Add(connection);

var stopwatch = Stopwatch.StartNew();
var counter = 0;
var users = Enumerable.Range(0, 200).Select(_ => CreateUser()).ToArray();

await Parallel.ForEachAsync(users, async (innerTask, _) =>
{
    Interlocked.Increment(ref counter);
    Console.Write("\rHandling connection no " + counter);

    await innerTask;
});

Console.WriteLine();
Console.WriteLine($"Created {connections.Count} connections in {stopwatch.Elapsed}");
stopwatch.Stop();

foreach (var con in connections)
{
    await con.DisposeAsync();
}

async Task CreateUser()
{
    await Task.Delay(Random.Shared.Next(0, 30_000));

    var userConnection = await HubConnector.CreateConnection(baseUrl, session.Id);

    if (await UserService.ConnectUser(baseUrl, session.Id, userConnection))
        connections.Add(userConnection);
}