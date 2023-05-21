using System.Net.Http.Json;
using System.Text.Json;
using Application.Serialization;
using Domain.Models;
using Domain.Simulations;
using Service.Integration.Models;
using Service.Models;
using Service.Models.Requests;
using Session = Service.Integration.Models.Session;

namespace Service.Integration.Extensions;

public static class HttpClientExtensions
{
    public static async Task<SessionControl> CreateSession(this HttpClient client)
    {
        var req = new SessionRequest { Name = "This is some super nice testing session" };
        var res = await client.PostAsJsonAsync("v1/sessions", req);
        var session = await res.Content.ReadFromJsonAsync<SessionControl>(Application.Defaults.Options);

        Assert.NotNull(session);

        return session;
    }

    public static async Task PrepareSession(this HttpClient client, SessionControl sessionControl,
        ISimulation simulation)
    {
        var req = new SessionActionRequest
        {
            ControlId = sessionControl.ControlId,
            Action = SessionActionDto.Prepare,
            Configuration = simulation.ToJsonElement()
        };
        var res = await client.PostAsJsonAsync($"v1/sessions/{sessionControl.Id}/actions", req);
        var session = await res.Content.ReadFromJsonAsync<Session>(Application.Defaults.Options);

        Assert.NotNull(session);
        Assert.Equal(SessionStatus.Preparing, session.Status);
    }

    public static async Task<UserControl> CreateUser(this HttpClient client, Session session, string userName = "Sarah")
    {
        var req = new UserRequest { Name = userName };
        var res = await client.PostAsJsonAsync($"v1/sessions/{session.Id}/users", req);
        var user = await res.Content.ReadFromJsonAsync<UserControl>(Application.Defaults.Options);

        Assert.NotNull(user);

        return user;
    }

    public static async Task UpdateUser(this HttpClient client, Guid sessionId, UserControl userControlControl,
        ISimulationUser configuration)
    {
        var req = new UserActionRequest
        {
            ControlId = userControlControl.ControlId,
            Status = UserStatusDto.Ready,
            Configuration = configuration.ToJsonElement()
        };
        var res = await client.PostAsJsonAsync($"v1/sessions/{sessionId}/users/{userControlControl.Id}/actions", req);
        var user = await res.Content.ReadFromJsonAsync<User>(Application.Defaults.Options);

        Assert.NotNull(user);
        Assert.Equal(UserStatus.Ready, user.Status);
    }

    public static async Task NotifySession(this HttpClient client, SessionControl session, JsonElement data)
    {
        var req = new MessageRequest
        {
            ControlId = session.ControlId,
            Text = data.ToString()
        };
        var res = await client.PostAsJsonAsync($"v1/sessions/{session.Id}/messages", req);
        var reSession = await res.Content.ReadFromJsonAsync<Session>(Application.Defaults.Options);

        Assert.NotNull(reSession);
    }

    public static async Task<Session> StartSession(this HttpClient client, SessionControl session,
        ISimulation? configuration = null)
    {
        var req = new SessionActionRequest
        {
            ControlId = session.ControlId,
            Action = SessionActionDto.Start,
            Configuration = configuration?.ToJsonElement()
        };
        var res = await client.PostAsJsonAsync($"v1/sessions/{session.Id}/actions", req);
        var reSession = await res.Content.ReadFromJsonAsync<Session>(Application.Defaults.Options);

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
        var reSession = await res.Content.ReadFromJsonAsync<Session>(Application.Defaults.Options);

        Assert.NotNull(reSession);

        return reSession;
    }

    public static async Task ExecuteUserAction(this HttpClient client, Session session, UserControl userControl,
        JsonElement data)
    {
        var req = new UserActionRequest
        {
            ControlId = userControl.ControlId,
            Configuration = data
        };
        var res = await client.PostAsJsonAsync($"v1/sessions/{session.Id}/users/{userControl.Id}/actions", req);
        var resUser = await res.Content.ReadFromJsonAsync<UserControl>(Application.Defaults.Options);

        Assert.NotNull(resUser);
    }
}