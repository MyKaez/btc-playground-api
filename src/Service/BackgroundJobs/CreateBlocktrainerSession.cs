using System.Text.Json;
using Application.Services;
using Domain.Models;

namespace Service.BackgroundJobs;

public class CreateBlocktrainerSession : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;


    public CreateBlocktrainerSession(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sessionService = _serviceProvider.GetRequiredService<ISessionService>();
        var userService = _serviceProvider.GetRequiredService<IUserService>();
        var sessions = await sessionService.GetAll(stoppingToken);
        var session = sessions.FirstOrDefault(s => s.Name == Extensions.SessionExtensions.BlocktrainerSessionName) ??
                      await sessionService.CreateSession(
                          Extensions.SessionExtensions.BlocktrainerSessionName, JsonDocument.Parse("{}").RootElement,
                          stoppingToken);

        if (session is null)
            throw new NotSupportedException("Cannot create Blocktrainer session");

        while (!stoppingToken.IsCancellationRequested)
        {
            var blockTrainerSession = await sessionService.GetById(session.Id, stoppingToken);

            if (blockTrainerSession is null)
                throw new NotSupportedException("Cannot find Blocktrainer session");

            var update = new SessionUpdate
                { SessionId = session.Id, Configuration = blockTrainerSession.Configuration!.Value };

            await sessionService.UpdateSession(update, stoppingToken);

            if (blockTrainerSession.Updated.AddMinutes(15) < DateTime.UtcNow)
            {
                update.Action = SessionAction.Reset;

                await sessionService.UpdateSession(update, stoppingToken);

                foreach (var user in await userService.GetBySessionId(session.Id, stoppingToken))
                    await sessionService.DeleteUser(session.Id, user.Id, stoppingToken);
            }

            await Task.Delay(60_000, stoppingToken);
        }
    }
}