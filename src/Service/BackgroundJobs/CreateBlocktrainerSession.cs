using System.Text.Json;
using Application.Services;
using Domain.Models;

namespace Service.BackgroundJobs;

public class CreateBlocktrainerSession : BackgroundService
{
    private readonly ISessionService _sessionService;

    public CreateBlocktrainerSession(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var session = await _sessionService.CreateSession(
            Extensions.SessionExtensions.BlocktrainerSessionName, JsonDocument.Parse("{}").RootElement, stoppingToken);

        if (session is null)
            throw new NotSupportedException("Cannot create Blocktrainer session");

        while (!stoppingToken.IsCancellationRequested)
        {
            var blockTrainerSession = await _sessionService.GetById(session.Id, stoppingToken);

            if (blockTrainerSession is null)
                throw new NotSupportedException("Cannot find Blocktrainer session");

            var update = new SessionUpdate { SessionId = session.Id, Configuration = blockTrainerSession.Configuration!.Value };

            await _sessionService.UpdateSession(update, stoppingToken);
            await Task.Delay(60_000, stoppingToken);
        }
    }
}