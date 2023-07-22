﻿using Application.Services;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Service.BackgroundJobs;

public class SessionKeepAliveUpdates : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public SessionKeepAliveUpdates(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(30_000, stoppingToken);

            var hubContext = _serviceProvider.GetRequiredService<IHubContext<SessionHub>>();
            var sessionService = _serviceProvider.GetRequiredService<ISessionService>();

            var sessions = await sessionService.GetAll(stoppingToken);

            foreach (var session in sessions)
                await hubContext.Clients.All.SendAsync(session.Id + ":SessionAlive", stoppingToken);
        }
    }
}