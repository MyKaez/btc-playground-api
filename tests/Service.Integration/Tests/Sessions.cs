using Microsoft.Extensions.DependencyInjection;
using Service.Integration.Extensions;
using Service.Integration.Models;
using Service.Integration.SingnalR;
using Shared;

namespace Service.Integration.Tests;

public class Sessions
{
    private readonly HttpClient _client;
    private readonly HubConnector _hubConnector;

    public Sessions()
    {
        var provider = ServiceProviderFactory.CreateServiceProvider();
        var httpClientFactory = provider.GetRequiredService<HttpClientFactory>();

        _client = httpClientFactory.CreateClient();
        _hubConnector = new HubConnector(httpClientFactory.TestServer);
    }

    [Fact]
    public async Task NotifyAboutCreatedUsers()
    {
        var session = await _client.CreateSession();
        var connection = await _hubConnector.CreateConnection(session);
        var users = await CreateUsers(session);
        var waitingForMessages = true;

        while (waitingForMessages)
        {
            if (_hubConnector.Messages.Count() == users.Count)
                waitingForMessages = false;

            await Task.Delay(100);
        }

        await connection.DisposeAsync();

        Assert.False(waitingForMessages);
    }

    private async Task<IReadOnlyCollection<User>> CreateUsers(Session session)
    {
        return new[]
        {
            await _client.CreateUser(session, "Kenny"),
            await _client.CreateUser(session, "Nico"),
            await _client.CreateUser(session, "Danny")
        };
    }
}