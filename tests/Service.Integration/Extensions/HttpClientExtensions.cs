using System.Net.Http.Json;
using System.Text.Json;
using Service.Integration.Models;
using Service.Models.Requests;

namespace Service.Integration.Extensions;

public static class HttpClientExtensions
{
    public static async Task<SessionControl> CreateSession(this HttpClient client)
    {
        var req = new SessionRequest { Name = "This is some super nice testing session" };
        var res = await client.PostAsJsonAsync("v1/sessions", req);
        var session = await res.Content.ReadFromJsonAsync<SessionControl>(Defaults.Options);

        Assert.NotNull(session);

        return session;
    }

    public static async Task<User> CreateUser(this HttpClient client, Session session, string userName = "Sarah")
    {
        var req = new UserRequest { Name = userName };
        var res = await client.PostAsJsonAsync($"v1/sessions/{session.Id}/users", req);
        var user = await res.Content.ReadFromJsonAsync<User>(Defaults.Options);

        Assert.NotNull(user);

        return user;
    }

    public static async Task<Session> NotifySession(this HttpClient client, SessionControl session, JsonElement data)
    {
        var req = new SessionActionRequest
        {
            ControlId = session.ControlId,
            Action = SessionActionDto.Notify,
            Data = data
        };
        var res = await client.PostAsJsonAsync($"v1/sessions/{session.Id}/actions", req);
        var reSession = await res.Content.ReadFromJsonAsync<Session>(Defaults.Options);

        Assert.NotNull(reSession);

        return reSession;
    }

    public static async Task<Session> StartSession(this HttpClient client, SessionControl session)
    {
        var req = new SessionActionRequest
        {
            ControlId = session.ControlId,
            Action = SessionActionDto.Start,
        };
        var res = await client.PostAsJsonAsync($"v1/sessions/{session.Id}/actions", req);
        var reSession = await res.Content.ReadFromJsonAsync<Session>(Defaults.Options);

        Assert.NotNull(reSession);

        return reSession;
    }
    
    public static async Task<Session> StopSession(this HttpClient client, SessionControl session)
    {
        var req = new SessionActionRequest
        {
            ControlId = session.ControlId,
            Action = SessionActionDto.Stop
        };
        var res = await client.PostAsJsonAsync($"v1/sessions/{session.Id}/actions", req);
        var reSession = await res.Content.ReadFromJsonAsync<Session>(Defaults.Options);

        Assert.NotNull(reSession);

        return reSession;
    }

    public static async Task ExecuteUserAction(this HttpClient client, Session session, User user, JsonElement data)
    {
        var req = new UserActionRequest
        {
            ControlId = user.ControlId,
            Data = data
        };
        var res = await client.PostAsJsonAsync($"v1/sessions/{session.Id}/users/{user.Id}/actions", req);
        var resUser = await res.Content.ReadFromJsonAsync<User>(Defaults.Options);

        Assert.NotNull(resUser);
    }
}