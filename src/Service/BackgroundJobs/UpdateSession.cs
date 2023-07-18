using Application.Services;

namespace Service.BackgroundJobs;

public class UpdateSession : BackgroundService
{
    private readonly ISessionService _sessionService;

    public UpdateSession(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            var sessions = await _sessionService.GetAll(stoppingToken);

            foreach (var session in sessions)
            {
                
            }

            await Task.Delay(5_000, stoppingToken);
        }
    }
}