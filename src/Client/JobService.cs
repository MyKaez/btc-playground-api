using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Client.Models;
using Polly;
using Service.Models.Requests;

namespace Client;

public class JobService
{
    private static readonly HashSet<string> Errors = new();

    private static readonly AsyncPolicy Policy = Polly.Policy.Handle<Exception>()
        .WaitAndRetryAsync(5, current => TimeSpan.FromMilliseconds(50 * (current + 1)), (exception, _) =>
        {
            if (Errors.Add(exception.Message))
                Console.WriteLine("Error, retrying:" + exception.Message);
        });

    public static async Task<ControlObject> CreateSession(string baseUrl)
    {
        return await Policy.ExecuteAsync(async () =>
        {
            var http = new HttpClient
            {
                BaseAddress = new Uri(baseUrl + "v1/"),
                Timeout = TimeSpan.FromSeconds(10)
            };

            var createSession = new SessionRequest
            {
                Name = "Kenny" + DateTime.Now,
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
        });
    }
}