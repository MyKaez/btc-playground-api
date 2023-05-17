using Domain.Models;

namespace Application.Services;

public interface IMessageService
{
    Task SendMessage(Guid sessionId, Message message, CancellationToken cancellationToken);
}