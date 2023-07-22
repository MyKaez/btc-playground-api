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

        Console.WriteLine(session.Id);
        Console.WriteLine(session.ControlId);

        return session;
    }
}