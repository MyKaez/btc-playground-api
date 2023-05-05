using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Client.Models;
using Service.Models.Requests;

namespace Client;

public class SessionFactory
{
    public async Task<Session> CreateSession()
    {
        var client = new HttpClient();
        var req = new SessionRequest { Name = "Kenny's test" };
        var res = await client.PostAsJsonAsync("https://localhost:5001/v1/sessions", req);
        var session = await res.Content.ReadFromJsonAsync<Session>(Defaults.Options);

        if (session is null)
            throw new NotSupportedException("No session created!");

        Console.WriteLine("created session: " + session.Id);

        return session;
    }
}