using System.Net.Http.Json;
using Service.Integration.Models;
using Service.Models.Requests;

namespace Service.Integration.Extensions;

public static class HttpClientExtensions
{
    public static async Task<Session> CreateSession(this HttpClient client)
    {
        var req = new SessionRequest { Name = "This is some super nice testing session" };
        var res = await client.PostAsJsonAsync("v1/sessions", req);
        var session = await res.Content.ReadFromJsonAsync<Session>(Defaults.Options);

        Assert.NotNull(session);

        return session;
    }

    public static async Task<User> CreateUser(this HttpClient client, Session session, string userName)
    {
        var req = new UserRequest { Name = userName };
        var res = await client.PostAsJsonAsync($"v1/sessions/{session.Id}/users", req);
        var user = await res.Content.ReadFromJsonAsync<User>(Defaults.Options);

        Assert.NotNull(user);

        return user;
    }
}