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
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    private readonly TestServer _testServer;

    public PowSimulation(ITestOutputHelper testOutputHelper)
    {
        var provider = ServiceProviderFactory.CreateServiceProvider();
        var httpClientFactory = provider.GetRequiredService<HttpClientFactory>();

        _client = httpClientFactory.CreateClient();
        _testServer = httpClientFactory.TestServer;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Simulate()
    {
        var sessionControl = await _client.CreateSession();
        var (connection, messages) = await _testServer.CreateHub(sessionControl);
        var hashingPower = EstimateHashesPerSecond(sessionControl);
        var users = await AddUsers(sessionControl, hashingPower);
        var configuration = await CreateConfig(sessionControl, users);
        var line = GetBlock(users, configuration);
        var hex = line.ComputeSha256Hash();
        var num = BigInteger.Parse(hex, NumberStyles.AllowHexSpecifier).ToString("0");

        // check correct hash
        // announce winner

        await connection.DisposeAsync();
    }

    private int EstimateHashesPerSecond(Session sessionControl)
    {
        const int fakeHashingPower = 100;
        
        var users = CreateUsers(sessionControl, fakeHashingPower)
            .ConfigureAwait(false).GetAwaiter().GetResult();
        var startTime = DateTime.Now.ToString("yyyyMMdd-HHmmssfff");
        var num = 0L;
        var useRanges = GetUserSettings(users, fakeHashingPower);
        var userHashCount = users.ToDictionary(u => u.User.Id, _ => 0);
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.ElapsedMilliseconds < 5_000)
        {
            num++;
            Hash(useRanges, startTime, userHashCount, out _);
        }

        var hashesPerSecond = (int)(num / 5);
        _testOutputHelper.WriteLine("Estimated hashing power: " + hashesPerSecond);
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
        var userRanges = GetUserSettings(users, totalHash);
        var startTime = DateTime.Now.ToString("yyyyMMdd-HHmmssfff");
        var userHashCount = users.ToDictionary(u => u.User.Id, _ => 0);
        var max = BigInteger.Pow(2, 256);
        var difficulty = new BigInteger(configuration.Difficulty!.Value);
        var threshold = max / difficulty;

        _testOutputHelper.WriteLine("total hashing power: " + totalHash);
        _testOutputHelper.WriteLine("difficulty: " + configuration.Difficulty);
        _testOutputHelper.WriteLine("expected: " + configuration.Expected);
        _testOutputHelper.WriteLine("seconds until block: " + configuration.SecondsUntilBlock);
        _testOutputHelper.WriteLine("expected hash: " + threshold.ToString("X").PadLeft(32, '0'));
        // _testOutputHelper.WriteLine("expected binary: " + threshold.);
        
        string line;
        var wasNegative = false;

        var stopwatch = Stopwatch.StartNew();
        
        while (true)
        {
            line = Hash(userRanges, startTime, userHashCount, out var numeric);

            if (numeric.Sign < 0)
                wasNegative = true;

            if (numeric <= threshold)
                break;
        }
        
        stopwatch.Stop();
        _testOutputHelper.WriteLine("Duration to find block: " + stopwatch.Elapsed);

        Assert.NotEmpty(line);
        Assert.False(wasNegative);

        return line;
    }

    private static string Hash(
        Dictionary<Guid, (double Start, double End)> userRanges, string uniqueIdentifier,
        IDictionary<Guid, int> userHashCount, out BigInteger numeric)
    {
        var random = Random.Shared.NextDouble();
        var userId = userRanges.FirstOrDefault(u => random >= u.Value.Start && random <= u.Value.End).Key;
        if (userId == Guid.Empty)
            userId = userRanges.Last().Key;
        var line = $"{userId}_{uniqueIdentifier}_{userHashCount[userId]}";
        userHashCount[userId] += 1;
        var hash = "0" + line.ComputeSha256Hash();
        numeric = BigInteger.Parse(hash, NumberStyles.AllowHexSpecifier);
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

    private async Task<(UserControl User, ProofOfWorkUser PowConfig)[]> AddUsers(SessionControl sessionControl,
        int hashingPower)
    {
        var users = await CreateUsers(sessionControl, hashingPower);

        foreach (var user in users)
            await _client.UpdateUser(sessionControl.Id, user.User, user.PowConfig);

        return users;
    }

    private async Task<(UserControl User, ProofOfWorkUser PowConfig)[]> CreateUsers(
        Session sessionControl, int hashingPower)
    {
        var first = Random.Shared.NextDouble();
        var second = Random.Shared.NextDouble();
        var third = Random.Shared.NextDouble();
        var total = first + second + third;

        var users = new[]
        {
            (await _client.CreateUser(sessionControl, "Kenny"),
                new ProofOfWorkUser { HashRate = (int)(hashingPower * (first / total)) }),
            (await _client.CreateUser(sessionControl, "Nico"),
                new ProofOfWorkUser { HashRate = (int)(hashingPower * (second / total)) }),
            (await _client.CreateUser(sessionControl, "Danny"),
                new ProofOfWorkUser { HashRate = (int)(hashingPower * (third / total)) })
        };

        return users;
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
        Assert.Equal(users.Sum(u => u.HashRate), update.TotalHashRate);

        return update;
    }
}