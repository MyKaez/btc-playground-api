using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using Application.Extensions;
using Application.Serialization;
using Domain.Simulations;
using Infrastructure.Simulations.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Service.Integration.Extensions;
using Service.Integration.Models;
using Service.Models;
using Shared;
using Xunit.Abstractions;

namespace Service.Integration.Tests;

public class PowSimulation
{
    private record UserConfig(Guid UserId, double Start, double End)
    {
        public int HashCount { get; set; }
    }

    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    private readonly TestServer _testServer;
    private readonly string _startTime;

    public PowSimulation(ITestOutputHelper testOutputHelper)
    {
        var provider = ServiceProviderFactory.CreateServiceProvider();
        var httpClientFactory = provider.GetRequiredService<HttpClientFactory>();

        _client = httpClientFactory.CreateClient();
        _testServer = httpClientFactory.TestServer;
        _testOutputHelper = testOutputHelper;
        _startTime = DateTime.Now.ToString("yyyyMMdd-HHmmssfff");
    }

    [Fact]
    public async Task Simulate()
    {
        var sessionControl = await _client.CreateSession();
        var (connection, messages) = await _testServer.CreateHub(sessionControl);
        var hashingPower = EstimateHashesPerSecond(sessionControl);
        var users = await AddUsers(sessionControl, hashingPower);
        var configuration = await CreateConfig(sessionControl, users);
        var block = GetBlock(users, configuration);
        var (user, _) = users.First(u => u.User.Id == block.UserId);
        user.Status = UserStatusDto.Done;

        await _client.ExecuteUserAction(sessionControl, user, block.ToJsonElement());
        await connection.DisposeAsync();

        Assert.Contains(messages, msg => msg.Contains(block.Hash));
        Assert.Contains(messages, msg => msg.Contains("stopped"));
    }

    private int EstimateHashesPerSecond(Session sessionControl)
    {
        const int fakeHashingPower = 100;

        var users = CreateUsers(sessionControl, fakeHashingPower)
            .ConfigureAwait(false).GetAwaiter().GetResult();
        var num = 0L;
        var useConfigs = GetUserSettings(users, fakeHashingPower).ToArray();
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.ElapsedMilliseconds < 5_000)
        {
            num++;
            Hash(useConfigs, out _, out _);
        }

        var hashesPerSecond = (int)(num / 5);
        _testOutputHelper.WriteLine("estimated hashing power: " + hashesPerSecond);
        return hashesPerSecond;
    }

    private async Task<ProofOfWorkSession> CreateConfig(
        SessionControl sessionControl, IEnumerable<(UserControl User, ProofOfWorkUser PowConfig)> users)
    {
        var configuration = await Prepare(sessionControl);
        var userSettings = users.Select(u => u.PowConfig).ToArray();
        configuration = await Start(sessionControl, configuration, userSettings);
        return configuration;
    }

    private ProofOfWorkBlock GetBlock((UserControl User, ProofOfWorkUser PowConfig)[] users, ProofOfWorkSession configuration)
    {
        var totalHash = users.Select(u => u.PowConfig.HashRate).Sum();
        var userConfigs = GetUserSettings(users, totalHash).ToArray();

        _testOutputHelper.WriteLine("total hashing power: " + totalHash);
        _testOutputHelper.WriteLine("seconds until block: " + configuration.SecondsUntilBlock);
        _testOutputHelper.WriteLine("difficulty: " + configuration.Difficulty);
        _testOutputHelper.WriteLine("expected: " + configuration.Expected);
        _testOutputHelper.WriteLine("threshold: " + configuration.Threshold);

        var line = "";
        var numeric = ProofOfWorkSession.Max;
        var userId = Guid.Empty;
        var threshold = BigInteger.Parse(configuration.Threshold!, NumberStyles.AllowHexSpecifier);
        var stopwatch = Stopwatch.StartNew();

        while (numeric >= threshold)
            line = Hash(userConfigs, out userId, out numeric);

        stopwatch.Stop();
        _testOutputHelper.WriteLine("Duration to find block: " + stopwatch.Elapsed);
        _testOutputHelper.WriteLine($"Found hash for line: {line}");
        _testOutputHelper.WriteLine($"Hash: {line.ComputeSha256Hash()}");

        Assert.NotEmpty(line);

        return new ProofOfWorkBlock(userId, line, line.ComputeSha256Hash());
    }

    private string Hash(IReadOnlyCollection<UserConfig> users, out Guid userId, out BigInteger numeric)
    {
        const string positiveSign = "0";
        var random = Random.Shared.NextDouble();
        var user = users.FirstOrDefault(u => random >= u.Start && random <= u.End) ?? users.Last();
        var line = $"{user.UserId}_{_startTime}_{user.HashCount++}";
        var hash = positiveSign + line.ComputeSha256Hash();
        userId = user.UserId;
        numeric = BigInteger.Parse(hash, NumberStyles.AllowHexSpecifier);
        return line;
    }

    private static IEnumerable<UserConfig> GetUserSettings(
        IEnumerable<(UserControl User, ProofOfWorkUser PowConfig)> users, long totalHash)
    {
        UserConfig? config = null;

        foreach (var user in users)
        {
            var turnPercentage = (double)user.PowConfig.HashRate / totalHash;
            var turnRangeStart = config?.End ?? 0;
            var turnRangeEnd = turnPercentage + turnRangeStart;

            yield return config = new UserConfig(user.User.Id, turnRangeStart, turnRangeEnd);
        }
    }

    private async Task<(UserControl User, ProofOfWorkUser PowConfig)[]> AddUsers(
        Session sessionControl, int hashingPower)
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

    private async Task<ProofOfWorkSession> Prepare(SessionControl sessionControl)
    {
        var configuration = new ProofOfWorkSession { SecondsUntilBlock = 10 };

        await _client.PrepareSession(sessionControl, configuration);

        return configuration;
    }

    private async Task<ProofOfWorkSession> Start(
        SessionControl sessionControl, ISimulation input, IEnumerable<ProofOfWorkUser> users)
    {
        var session = await _client.StartSession(sessionControl, input);

        Assert.NotNull(session);

        var update = session.Configuration.Deserialize<ProofOfWorkSession>(Application.Defaults.Options);

        Assert.NotNull(update);
        Assert.NotNull(update.Difficulty);
        Assert.NotNull(update.Expected);
        Assert.Equal(users.Sum(u => u.HashRate), update.TotalHashRate);

        return update;
    }
}