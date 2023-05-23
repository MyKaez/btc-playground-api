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

    public async Task<RequestResult<JsonElement>?> SessionPrepare(Session session, JsonElement config,
        CancellationToken cancellationToken)
    {
        var preparation = new ProofOfWorkSession { SecondsUntilBlock = 10 };
        var sessionUpdate = new SessionUpdate
        {
            SessionId = session.Id,
            Action = SessionAction.Stop,
            Configuration = preparation.ToJsonElement()
        };

        await _sessionService.UpdateSession(sessionUpdate, cancellationToken);

        return new RequestResult<JsonElement>(sessionUpdate.Configuration);
    }

    public async Task<RequestResult<JsonElement>?> SessionStart(Session session, JsonElement config,
        CancellationToken cancellationToken)
    {
        var users = await _userService.GetBySessionId(session.Id, cancellationToken);
        var userConfigs = users
            .Where(u => u.Status == UserStatus.Ready)
            .Select(u => u.Configuration)
            .Where(u => u.HasValue)
            .Select(c => c?.FromJsonElement<ProofOfWorkUser>()!)
            .ToArray();
        var sessionConfig = config.FromJsonElement<ProofOfWorkSession>();

        if (sessionConfig is null)
            throw new NotSupportedException();

        ProofOfWorkSession.Calculate(sessionConfig, userConfigs.Sum(c => c.HashRate));

        var res = sessionConfig.ToJsonElement();

        return new RequestResult<JsonElement>(res);
    }

    public async Task<RequestResult<JsonElement>?> SessionReset(Session session, JsonElement config, CancellationToken cancellationToken)
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

        return new RequestResult<JsonElement>(JsonDocument.Parse("{}").RootElement);
    }

    public async Task<RequestResult<JsonElement>?> UserReady(
        Session session, User user, JsonElement config, CancellationToken cancellationToken)
    {
        var preparation = session.Configuration?.FromJsonElement<ProofOfWorkSession>()!;
        var userConfig = config.FromJsonElement<ProofOfWorkUser>();
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

        return new RequestResult<JsonElement>(config);
    }

    public async Task<RequestResult<JsonElement>?> UserDone(Session session, JsonElement config,
        CancellationToken cancellationToken)
    {
        var block = config.Deserialize<ProofOfWorkBlock>(Application.Defaults.Options);

        if (block is null)
            throw new NotSupportedException();

        if (!string.Equals(block.Text.ComputeSha256Hash(), block.Hash, StringComparison.CurrentCultureIgnoreCase))
            return new RequestResult<JsonElement>(new BadRequest("The text does not match to the hash"));

        var threshold = session.Configuration?.FromJsonElement<ProofOfWorkSession>()?.Threshold!;

        if (string.Compare(block.Hash, threshold, StringComparison.CurrentCultureIgnoreCase) > 0)
            return new RequestResult<JsonElement>(new BadRequest("The hash is larger than the threshold"));

        var sessionUpdate = new SessionUpdate
        {
            SessionId = session.Id,
            Action = SessionAction.Stop,
            Configuration = config
        };

        await _sessionService.UpdateSession(sessionUpdate, cancellationToken);

        return new RequestResult<JsonElement>(config);
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