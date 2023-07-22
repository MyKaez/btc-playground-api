using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Client.Models;
using Service.Models.Requests;

namespace Client;

public class JobService
{
    public static async Task<ControlObject> CreateSession(string baseUrl)
    {
        var http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl + "v1/"),
            Timeout = TimeSpan.FromSeconds(10)
        };

        var createSession = new SessionRequest
        {
            Name = "Kenny",
            Configuration = null
        };
        var response = await http.PostAsJsonAsync("sessions", createSession);
        
        if (response.StatusCode != HttpStatusCode.OK)
            Console.WriteLine(response.StatusCode);
        
        var opt = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var session = await response.Content.ReadFromJsonAsync<ControlObject>(opt);

        Console.WriteLine(session!.Id);
        Console.WriteLine(session.ControlId);

        return session;
    }
}