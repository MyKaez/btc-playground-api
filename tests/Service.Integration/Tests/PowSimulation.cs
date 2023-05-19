using Domain.Simulations;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Service.Integration.Extensions;
using Shared;

namespace Service.Integration.Tests;

public class PowSimulation
{
    private readonly HttpClient _client;
    private readonly TestServer _testServer;

    public PowSimulation()
    {
        var provider = ServiceProviderFactory.CreateServiceProvider();
        var httpClientFactory = provider.GetRequiredService<HttpClientFactory>();

        _client = httpClientFactory.CreateClient();
        _testServer = httpClientFactory.TestServer;
    }

    [Fact]
    public async Task X()
    {
        var sessionControl = await _client.CreateSession();
        var (connection, messages) = await _testServer.CreateHub(sessionControl);
        var users = new[]
        {
            await _client.CreateUser(sessionControl, "Kenny"),
            await _client.CreateUser(sessionControl, "Nico"),
            await _client.CreateUser(sessionControl, "Danny")
        };
        var configuration = new ProofOfWork { SecondsUntilBlock = 10 };

        await _client.PrepareSession(sessionControl, configuration);

        foreach (var user in users)
        {
            await _client.UpdateUser(sessionControl.Id, user, new ProofOfWorkUser { HashRate = Random.Shared.Next() });
        }

        var session = await _client.StartSession(sessionControl);

        // get pow config
        // user start hashing
        // check correct hash
        // announce winner

        await connection.DisposeAsync();
    }
}