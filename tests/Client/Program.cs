using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Client.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Service.Models.Requests;

var client = new HttpClient();
var req = new SessionRequest { Name = "Kenny's test" };
var res = await client.PostAsJsonAsync("https://localhost:5001/sessions", req);
var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
var session = await res.Content.ReadFromJsonAsync<Session>(options);

if (session is null)
    throw new NotSupportedException("No session created!");

Console.WriteLine("created session: " + session.Id);


var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:5001/session-hub")
    .Build();

connection.On<string>(
    session.Id.ToString(),
    message => Console.WriteLine($"Received message: {message}"));

await connection.StartAsync();

Console.WriteLine("SignalR connection started.");


var req2 = new UserRequest { Name = "newa002" };
await client.PostAsJsonAsync($"https://localhost:5001/sessions/{session.Id}/users", req2);


while (true)
{
    Console.Write("Enter message (or 'exit' to quit): ");
    var input = Console.ReadLine();

    if (input == "exit")
        break;

    await connection.InvokeAsync("SendMessage", input);
}

await connection.DisposeAsync();