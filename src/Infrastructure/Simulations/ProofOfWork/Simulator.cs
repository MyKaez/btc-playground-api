using Application.Serialization;
using System.Text.Json;
using Application.Extensions;
using Application.Models;
using Application.Services;
using Application.Simulations;
using Domain.Models;
using Infrastructure.Simulations.Models;

namespace Infrastructure.Simulations.ProofOfWork;

public class Simulator : ISimulator
{
    private readonly IUserService _userService;
    private readonly ISessionService _sessionService;

    public Simulator(IUserService userService, ISessionService sessionService)
    {
        _userService = userService;
        _sessionService = sessionService;
    }

    public Task<Result<JsonElement>> SessionPrepare(
        Session session, JsonElement config, CancellationToken cancellationToken)
    {
        var preparation = new ProofOfWorkSession { SecondsUntilBlock = 10 };

        return Task.FromResult<Result<JsonElement>>(preparation.ToJsonElement());
    }

    public async Task<Result<JsonElement>> SessionStart(
        Session session, JsonElement config, CancellationToken cancellationToken)
    {
        var sessionConfig = config.FromJsonElement<ProofOfWorkSession>();

        if (sessionConfig is null)
            throw new NotSupportedException();

        var users = await _userService.GetBySessionId(session.Id, cancellationToken);
        var userConfigs = users
            .Where(u => u.Status == UserStatus.Ready)
            .Select(u => u.Configuration)
            .Where(u => u.HasValue)
            .Select(c => c?.FromJsonElement<ProofOfWorkUser>()!)
            .ToArray();

        ProofOfWorkSession.Calculate(sessionConfig, userConfigs.Sum(c => c.HashRate));
        
        return sessionConfig.ToJsonElement();
    }

    public Task<Result<JsonElement>> SessionStop(
        Session session, JsonElement config, CancellationToken cancellationToken)
    {
        var sessionConfig = config.FromJsonElement<ProofOfWorkSession>();

        if (sessionConfig is null)
            throw new NotSupportedException();

        return Task.FromResult<Result<JsonElement>>(sessionConfig.ToJsonElement());
    }

    public async Task<Result<JsonElement>> SessionReset(
        Session session, JsonElement config, CancellationToken cancellationToken)
    {
        var users = await _userService.GetBySessionId(session.Id, cancellationToken);

        foreach (var user in users)
        {
            var update = new UserUpdate
            {
                SessionId = session.Id,
                UserId = user.Id,
                Status = UserStatus.NotReady,
                Configuration = JsonDocument.Parse("{}").RootElement
            };

            await _userService.Update(update, cancellationToken);
        }

        return JsonDocument.Parse("{}").RootElement;
    }

    public Task<Result<JsonElement>> UserNotReady(
        Session session, JsonElement config, CancellationToken cancellationToken)
    {
        return Task.FromResult<Result<JsonElement>>(JsonDocument.Parse("{}").RootElement);
    }

    public async Task<Result<JsonElement>> UserReady(
        Session session, User user, JsonElement config, CancellationToken cancellationToken)
    {
        var preparation = session.Configuration?.FromJsonElement<ProofOfWorkSession>()!;
        var userConfig = config.FromJsonElement<ProofOfWorkUser>();

        if (userConfig is null)
            throw new NotSupportedException();
        
        var sessionUsers = await _userService.GetBySessionId(session.Id, cancellationToken);
        var restUsers = sessionUsers
            .Where(u => u.Id != user.Id)
            .Where(u => u.Status == UserStatus.Ready)
            .Sum(r => r.Configuration?.FromJsonElement<ProofOfWorkUser>()?.HashRate ?? 0);

        ProofOfWorkSession.Calculate(preparation, restUsers + userConfig!.HashRate);

        var sessionUpdate = new SessionUpdate
        {
            SessionId = session.Id,
            Configuration = preparation.ToJsonElement()
        };

        await _sessionService.UpdateSession(sessionUpdate, cancellationToken);

        return config;
    }

    public async Task<Result<JsonElement>> UserDone(
        Session session, JsonElement config, CancellationToken cancellationToken)
    {
        var block = config.Deserialize<ProofOfWorkBlock>(Application.Defaults.Options);

        if (block is null)
            throw new NotSupportedException();

        if (!string.Equals(block.Text.ComputeSha256Hash(), block.Hash, StringComparison.CurrentCultureIgnoreCase))
            throw new NotSupportedException("The text does not match to the hash");

        var threshold = session.Configuration?.FromJsonElement<ProofOfWorkSession>()?.Threshold!;

        if (string.Compare(block.Hash, threshold, StringComparison.CurrentCultureIgnoreCase) > 0)
            throw new NotSupportedException("The hash is larger than the threshold");

        var sessionUpdate = new SessionUpdate
        {
            SessionId = session.Id,
            Action = SessionAction.Stop,
            Configuration = config
        };

        await _sessionService.UpdateSession(sessionUpdate, cancellationToken);

        return config;
    }

    public async Task UserDelete(Session session, Guid userId, CancellationToken cancellationToken)
    {
        var preparation = session.Configuration?.FromJsonElement<ProofOfWorkSession>()!;
        var sessionUsers = await _userService.GetBySessionId(session.Id, cancellationToken);
        var restUsers = sessionUsers
            .Where(u => u.Id != userId)
            .Where(u => u.Status == UserStatus.Ready)
            .Sum(r => r.Configuration?.FromJsonElement<ProofOfWorkUser>()?.HashRate ?? 0);

        ProofOfWorkSession.Calculate(preparation, restUsers);

        var sessionUpdate = new SessionUpdate
        {
            SessionId = session.Id,
            Configuration = preparation.ToJsonElement()
        };

        await _sessionService.UpdateSession(sessionUpdate, cancellationToken);
    }
}