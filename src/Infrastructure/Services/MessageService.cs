using Application.Services;
using Domain.Models;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly IHubContext<SessionHub> _hubContext;

    public MessageService(IHubContext<SessionHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessage(Guid sessionId, Message message, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.SendAsync(sessionId + ":UserMessage", message, cancellationToken);
    }
}