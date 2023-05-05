using System.Net.Http.Json;
using Client;
using Microsoft.AspNetCore.SignalR.Client;
using Service.Models.Requests;

var sessionFactory = new SessionFactory();
var session = await sessionFactory.CreateSession();
var hubConnector = new HubConnector(session);
var connection = await hubConnector.CreateConnection();


var client = new HttpClient();
var req2 = new UserRequest { Name = "kny" };

await client.PostAsJsonAsync($"https://localhost:5001/v1/sessions/{session.Id}/users", req2);
await client.PostAsJsonAsync($"https://localhost:5001/v1/sessions/{session.Id}/users", req2 with { Name = "nco" });
await client.PostAsJsonAsync($"https://localhost:5001/v1/sessions/{session.Id}/users", req2 with { Name = "dan" });

while (true)
{
    Console.WriteLine("Enter message (or 'exit' to quit): ");
    var input = Console.ReadLine();

    if (input == "exit")
        break;

    await connection.InvokeAsync("SendMessage", session.Id, input);
}

await connection.DisposeAsync();