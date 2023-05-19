using System.Text.Json;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Service.Integration.Extensions;
using Shared;

namespace Service.Integration.Tests;

public class NotifyUsers
{
    private readonly HttpClient _client;
    private readonly TestServer _testServer;

    public NotifyUsers()
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
        var (connection, messages) = await _testServer.CreateHub(session);
        var data = GetData();

        await _client.NotifySession(session, data);
        await connection.DisposeAsync();

        Assert.Contains(messages, msg => IsMessageCorrect(data, msg));
    }

    [Fact]
    public async Task UserSendsData_DataGetsShared()
    {
        var session = await _client.CreateSession();
        var (connection, messages) = await _testServer.CreateHub(session);
        var user = await _client.CreateUser(session);
        var data = GetData();

        await _client.ExecuteUserAction(session, user, data);
        await connection.DisposeAsync();

        Assert.Contains(messages, msg => msg.Contains(user.Name));
        Assert.Contains(messages, msg => IsMessageCorrect(data, msg));
    }

    private static bool IsMessageCorrect(JsonElement data, string msg)
    {
        var enumerator = data.EnumerateObject();
        var isCorrect = true;

        while (enumerator.MoveNext() && isCorrect)
        {
            isCorrect &= msg.Contains(enumerator.Current.Name) &&
                         msg.Contains(enumerator.Current.Value.GetRawText().Trim('"'));
        }

        return isCorrect;
    }

    private static JsonElement GetData()
    {
        var hash = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        var data = JsonDocument.Parse($$"""
        {
          "completed": true,
          "foundHash": "{{hash}}"
        }
        """);
        return data.RootElement;
    }
}