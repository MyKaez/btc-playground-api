using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Service.Integration.Extensions;
using Service.Integration.Models;
using Shared;

namespace Service.Integration.Tests;

public class NotifySessions
{
    private readonly HttpClient _client;
    private readonly TestServer _testServer;

    public NotifySessions()
    {
        var provider = ServiceProviderFactory.CreateServiceProvider();
        var httpClientFactory = provider.GetRequiredService<HttpClientFactory>();

        _client = httpClientFactory.CreateClient();
        _testServer = httpClientFactory.TestServer;
    }

    [Fact]
    public async Task MultipleUsers_NotifyAboutCreatedUsers()
    {
        var session = await _client.CreateSession();
        var (connection, messages) = await _testServer.CreateHub(session);
        var users = new[]
        {
            await _client.CreateUser(session, "Kenny"),
            await _client.CreateUser(session, "Nico"),
            await _client.CreateUser(session, "Danny")
        };
        var foundMessages = await FoundMessages(messages, users.ToArray());

        await connection.DisposeAsync();

        Assert.True(foundMessages);
    }

    [Fact]
    public async Task MultipleConnections_NotifyAllConnections()
    {
        var session = await _client.CreateSession();
        var hubs = new[]
        {
            await _testServer.CreateHub(session),
            await _testServer.CreateHub(session),
            await _testServer.CreateHub(session)
        };
        var user = await _client.CreateUser(session);

        foreach (var (connection, messages) in hubs)
        {
            var foundMessages = await FoundMessages(messages, user);

            Assert.True(foundMessages);

            await connection.DisposeAsync();
        }
    }

    private static async Task<bool> FoundMessages(
        IReadOnlyCollection<string> messages, params User[] users)
    {
        var waitingForMessages = true;

        for (var i = 0; i < 3; i++)
        {
            if (messages.Count == users.Length && users.All(user => messages.Any(msg => msg.Contains(user.Name))))
                waitingForMessages = false;

            await Task.Delay(100);
        }

        return !waitingForMessages;
    }
}