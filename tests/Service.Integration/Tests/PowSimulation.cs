using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using Application.Extensions;
using Domain.Simulations;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Service.Integration.Extensions;
using Service.Integration.Models;
using Shared;
using Xunit.Abstractions;

namespace Service.Integration.Tests;

public class PowSimulation
{
    private const int Xx = 100;

    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    private readonly TestServer _testServer;

    private int _hashingPower;

    public PowSimulation(ITestOutputHelper testOutputHelper)
    {
        var provider = ServiceProviderFactory.CreateServiceProvider();
        var httpClientFactory = provider.GetRequiredService<HttpClientFactory>();

        _client = httpClientFactory.CreateClient();
        _testServer = httpClientFactory.TestServer;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task X()
    {
        var sessionControl = await _client.CreateSession();
        var (connection, messages) = await _testServer.CreateHub(sessionControl);
        _hashingPower = EstimateHashesPerSecond(sessionControl);
        var users = await AddUsers(sessionControl);
        var configuration = await CreateConfig(sessionControl, users);

        var stopwatch = Stopwatch.StartNew();
        var line = GetBlock(users, configuration);

        Assert.NotEmpty(line);
        stopwatch.Stop();
        _testOutputHelper.WriteLine("Milliseconds to find block: " + stopwatch.ElapsedMilliseconds);

        var hex = line.ComputeSha256Hash();
        var num = BigInteger.Parse(hex, NumberStyles.AllowHexSpecifier).ToString("0");

        // check correct hash
        // announce winner

        await connection.DisposeAsync();
    }

    private int EstimateHashesPerSecond(SessionControl sessionControl)
    {
        var users = AddUsers(sessionControl).ConfigureAwait(false).GetAwaiter().GetResult();
        var now = DateTime.Now;
        var num = 0L;
        var dic = GetUserSettings(users, Xx);
        var dic2 = users.ToDictionary(u => u.User.Id, _ => 0);
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.ElapsedMilliseconds < 5_000)
        {
            var line = $"{now}_{num++}";
            var random = Random.Shared.NextDouble();
            var userId = dic.FirstOrDefault(u => random >= u.Value.Start && random <= u.Value.End).Key;
            dic2[userId] += 1;
            var hash = "0" + line.ComputeSha256Hash();
            BigInteger.Parse(hash, NumberStyles.AllowHexSpecifier);
        }

        var hashesPerSecond = (int)(num / 5);
        _testOutputHelper.WriteLine($"Estimated hashing power: " + hashesPerSecond);
        return hashesPerSecond;
    }

    private async Task<ProofOfWork> CreateConfig(SessionControl sessionControl,
        (UserControl User, ProofOfWorkUser PowConfig)[] users)
    {
        var configuration = await Prepare(sessionControl);
        var userSettings = users.Select(u => u.PowConfig).ToArray();
        configuration = await Start(sessionControl, configuration, userSettings);
        return configuration;
    }

    private string GetBlock((UserControl User, ProofOfWorkUser PowConfig)[] users, ProofOfWork configuration)
    {
        var totalHash = users.Select(u => u.PowConfig.HashRate).Sum();
        var dic = GetUserSettings(users, totalHash);
        var startTime = DateTime.Now.ToString("yyyyMMdd-HHmmssfff");
        var dic2 = users.ToDictionary(u => u.User.Id, _ => 0);

        string line;

        var max = BigInteger.Pow(2, 256);
        var difficulty = new BigInteger(configuration.Difficulty!.Value);
        var threshold = max / difficulty;

        _testOutputHelper.WriteLine("total hashing power: " + totalHash);
        _testOutputHelper.WriteLine("difficulty: " + configuration.Difficulty);
        _testOutputHelper.WriteLine("expected: " + configuration.Expected);
        _testOutputHelper.WriteLine("seconds until block: " + configuration.SecondsUntilBlock);
        _testOutputHelper.WriteLine("expected hash: " + threshold.ToString("X").PadLeft(32, '0'));
        // _testOutputHelper.WriteLine("expected binary: " + threshold.);

        var wasNegative = false;

        while (true)
        {
            var random = Random.Shared.NextDouble();
            var userId = dic.FirstOrDefault(u => random >= u.Value.Start && random <= u.Value.End).Key;
            if (userId == Guid.Empty)
                userId = dic.Last().Key;
            line = $"{userId}_{startTime}_{dic2[userId]}";
            dic2[userId] += 1;
            var hash = "0" + line.ComputeSha256Hash();
            var numeric = BigInteger.Parse(hash, NumberStyles.AllowHexSpecifier);

            if (numeric.Sign < 0)
                wasNegative = true;

            if (numeric <= threshold)
                break;
        }

        Assert.False(wasNegative);

        return line;
    }

    private static Dictionary<Guid, (double Start, double End)> GetUserSettings(
        (UserControl User, ProofOfWorkUser PowConfig)[] users, long totalHash)
    {
        var dic = new Dictionary<Guid, (double Start, double End)>();

        foreach (var user in users)
        {
            var tunPercentage = (double)user.PowConfig.HashRate / totalHash;
            var turnRangeStart = dic.LastOrDefault().Value.End;
            var turnRangeEnd = (tunPercentage + turnRangeStart);

            dic.Add(user.User.Id, (turnRangeStart, turnRangeEnd));
        }

        return dic;
    }

    private async Task<(UserControl User, ProofOfWorkUser PowConfig)[]> AddUsers(SessionControl sessionControl)
    {
        var users = await CreateUsers(sessionControl);

        foreach (var user in users)
            await _client.UpdateUser(sessionControl.Id, user.User, user.PowConfig);

        return users;
    }

    private async Task<(UserControl User, ProofOfWorkUser PowConfig)[]> CreateUsers(SessionControl sessionControl)
    {
        if (_hashingPower == 0)
        {
            var users = new[]
            {
                (await _client.CreateUser(sessionControl, "Kenny"),
                    new ProofOfWorkUser { HashRate = Random.Shared.Next(100, 999) }),
                (await _client.CreateUser(sessionControl, "Nico"),
                    new ProofOfWorkUser { HashRate = Random.Shared.Next(100, 999) }),
                (await _client.CreateUser(sessionControl, "Danny"),
                    new ProofOfWorkUser { HashRate = Random.Shared.Next(100, 999) })
            };

            return users;
        }
        else
        {
            var first = Random.Shared.NextDouble();
            var second = Random.Shared.NextDouble();
            var third = Random.Shared.NextDouble();
            var total = first + second + third;

            var users = new[]
            {
                (await _client.CreateUser(sessionControl, "Kenny"),
                    new ProofOfWorkUser { HashRate = (int)(_hashingPower * (first / total)) }),
                (await _client.CreateUser(sessionControl, "Nico"),
                    new ProofOfWorkUser { HashRate = (int)(_hashingPower * (second / total)) }),
                (await _client.CreateUser(sessionControl, "Danny"),
                    new ProofOfWorkUser { HashRate = (int)(_hashingPower * (third / total)) })
            };

            return users;
        }
    }

    private async Task<ProofOfWork> Prepare(SessionControl sessionControl)
    {
        var configuration = new ProofOfWork { SecondsUntilBlock = 10 };

        await _client.PrepareSession(sessionControl, configuration);

        return configuration;
    }

    private async Task<ProofOfWork> Start(
        SessionControl sessionControl, ProofOfWork input, ProofOfWorkUser[] users)
    {
        var session = await _client.StartSession(sessionControl, input);

        Assert.NotNull(session);

        var update = session.Configuration.Deserialize<ProofOfWork>(Application.Defaults.Options);

        Assert.NotNull(update);
        Assert.NotNull(update.Difficulty);
        Assert.NotNull(update.Expected);
        // todo: currently this is false because of the users set while doing the hashrate estimate
        // Assert.Equal(users.Sum(u => u.HashRate), update.TotalHashRate);

        return update;
    }
}