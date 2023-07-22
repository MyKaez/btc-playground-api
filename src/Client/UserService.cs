using System.Net.Http.Json;
using Client.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Service.Models.Requests;

namespace Client;

public class UserService
{
    public static async Task<bool> ConnectUser(string baseUrl, Guid sessionId, HubConnection userConnection)
    {
        var c = 0;
        while (true)
        {
            try
            {
                var userHttp = new HttpClient
                {
                    BaseAddress = new Uri(baseUrl + "v1/sessions/"),
                    Timeout = TimeSpan.FromSeconds(10)
                };
                var user = new UserRequest { Name = "Kenny" + Guid.NewGuid().ToString().Split('-')[0] };
                var userRes = await userHttp.PostAsJsonAsync($"{sessionId}/users", user);
                var userX = await userRes.Content.ReadFromJsonAsync<ControlObject>();

                await userConnection.InvokeCoreAsync("RegisterUser", new object[] { userX.Id });

                userConnection.KeepAliveInterval = TimeSpan.FromSeconds(10);
                userConnection.ServerTimeout = TimeSpan.FromMinutes(10);

                return true;
            }
            catch (Exception e) when (c++ < 3)
            {
                Console.WriteLine(e.Message);
                c++;
            }
            catch
            {
                break;
            }
        }

        return false;
    }
}