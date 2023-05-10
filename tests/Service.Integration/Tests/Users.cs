using Application.Extensions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Service.Integration.Extensions;
using Service.Integration.Models;
using Shared;

namespace Service.Integration.Tests;

public class Users
{
    private readonly HttpClient _client;
    private readonly TestServer _testServer;

    public Users()
    {
        var provider = ServiceProviderFactory.CreateServiceProvider();
        var httpClientFactory = provider.GetRequiredService<HttpClientFactory>();

        _client = httpClientFactory.CreateClient();
        _testServer = httpClientFactory.TestServer;
    }

    [Fact]
    public async Task SessionSendsData_DataGetsShared()
    {
        var session = await _client.CreateSession();
        var (connection, messages) = await CreateHub(session);
        var data = GetData();

        await _client.ExecuteSessionNotification(session, data);
        await connection.DisposeAsync();
        
        Assert.Contains(messages, msg => msg.Contains(data.JoinToString()));
    }

    [Fact]
    public async Task UserSendsData_DataGetsShared()
    {
        var session = await _client.CreateSession();
        var (connection, messages) = await CreateHub(session);
        var user = await _client.CreateUser(session);
        var data = GetData();
        
        await _client.ExecuteUserAction(session, user, data);
        await connection.DisposeAsync();

        Assert.Contains(messages, msg => msg.Contains(user.Name));
        Assert.Contains(messages, msg => msg.Contains(data.JoinToString()));
    }

    private async Task<TestHub> CreateHub(Session session)
    {
        var hubConnector = new HubConnector(_testServer);
        var hub = await hubConnector.CreateConnection(session);

        return hub;
    }

    private static IReadOnlyDictionary<string, object> GetData()
    {
        var data = new Dictionary<string, object>
        {
            { "completed", true },
            { "foundHash", Guid.Empty.ToString("N") + Guid.NewGuid().ToString("N") }
        };
        return data;
    }
}