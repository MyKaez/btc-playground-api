using Application.Serialization;
using System.Text.Json;
using Application.Models;
using Application.Services;
using Application.Simulations;
using Domain.Models;
using Domain.Simulations;

namespace Infrastructure.Simulations.ProofOfWork;

public class Simulator : ISimulator
{
    private readonly IUserService _userService;

    public Simulator(IUserService userService)
    {
        _userService = userService;
    }

    public Task<RequestResult<JsonElement>?> Prepare(Session session, CancellationToken cancellationToken)
    {
        return Task.FromResult<RequestResult<JsonElement>?>(null);
    }

    public async Task<RequestResult<JsonElement>?> Start(Session session, CancellationToken cancellationToken)
    {
        var users = await _userService.GetBySessionId(session.Id, cancellationToken);
        var userConfigs = users
            .Where(u => u.Status == UserStatus.Ready)
            .Select(u => u.Configuration)
            .Where(u => u.HasValue)
            .Select(c => c?.FromJsonElement<ProofOfWorkUser>()!)
            .ToArray();
        var sessionConfig = session.Configuration?.FromJsonElement<ProofOfWorkSession>();

        if (sessionConfig is null)
            throw new NotSupportedException();

        ProofOfWorkSession.Calculate(sessionConfig, userConfigs.Sum(c => c.HashRate));

        var config = sessionConfig.ToJsonElement();

        return new RequestResult<JsonElement>(config);
    }

    public Task<RequestResult<JsonElement>?> Stop(Session session, CancellationToken cancellationToken)
    {
        return Task.FromResult<RequestResult<JsonElement>?>(null);
    }

    public Task<RequestResult<JsonElement>?> Reset(Session session, CancellationToken cancellationToken)
    {
        return Task.FromResult<RequestResult<JsonElement>?>(null);
    }
}