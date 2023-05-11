using Domain.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Service.Integration.Extensions;
using Shared;

namespace Service.Integration.Tests;

public class StartAndStopSessions
{
    private readonly HttpClient _client;
    private readonly TestServer _testServer;

    public StartAndStopSessions()
    {
        var provider = ServiceProviderFactory.CreateServiceProvider();
        var httpClientFactory = provider.GetRequiredService<HttpClientFactory>();

        _client = httpClientFactory.CreateClient();
        _testServer = httpClientFactory.TestServer;
    }

    [Fact]
    public async Task Execute()
    {
        var session = await _client.CreateSession();
        var (connection, messages) = await _testServer.CreateHub(session);
        Assert.Equal(SessionStatus.NotStarted, session.Status);

        var startedSession = await _client.StartSession(session);
        Assert.Equal(session.Id, startedSession.Id);
        Assert.Equal(SessionStatus.Started, startedSession.Status);

        var stoppedSession = await _client.StopSession(session);
        Assert.Equal(session.Id, stoppedSession.Id);
        Assert.Equal(SessionStatus.Stopped, stoppedSession.Status);

        await connection.DisposeAsync();
        Assert.Equal(2, messages.Count);
        Assert.Contains(messages, val => val.Contains("Started"));
        Assert.Contains(messages, val => val.Contains("Stopped"));
    }
}