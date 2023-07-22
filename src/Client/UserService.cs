using System.Net;
using System.Net.Http.Json;
using Client.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Service.Models.Requests;

namespace Client;

public class UserService
{
    private static readonly HashSet<string> Errors = new();

    public static async Task<bool> ConnectUser(string baseUrl, Guid sessionId, HubConnection userConnection)
    {
        var c = 0;
        
        while (true)
        {
            try
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
            }
            catch (Exception e) when (c++ < 3)
            {
                if (Errors.Add(e.Message))
                    Console.WriteLine(e.Message);
                c++;
            }
            catch
            {
                return false;
            }
        }
    }
}