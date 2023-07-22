using System.Net;
using System.Net.Http.Json;
using Client.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Polly;
using Service.Models.Requests;

namespace Client;

public class UserService
{
    private static readonly HashSet<string> Errors = new();

    private static readonly AsyncPolicy Policy = Polly.Policy.Handle<Exception>()
        .WaitAndRetryAsync(5, current => TimeSpan.FromMilliseconds(50 * (current + 1)), (exception, _) =>
        {
            if (Errors.Add(exception.Message))
                Console.WriteLine("Error, retrying:" + exception.Message);
        });
    
    public static async Task<bool> ConnectUser(string baseUrl, Guid sessionId, HubConnection userConnection)
    {
        return await Policy.ExecuteAsync(async () =>
        {
            var http = new HttpClient
            {
                BaseAddress = new Uri(baseUrl + "v1/sessions/"),
                Timeout = TimeSpan.FromSeconds(10)
            };
            var createUser = new UserRequest { Name = "Kenny" + Guid.NewGuid().ToString().Split('-')[0] };
            var response = await http.PostAsJsonAsync($"{sessionId}/users", createUser);

            if (response.StatusCode != HttpStatusCode.OK)
                Console.WriteLine(response.StatusCode);

            var user = await response.Content.ReadFromJsonAsync<ControlObject>();

            await userConnection.InvokeCoreAsync("RegisterUser", new object[] { user!.Id });

            return true;
        });
    }
}