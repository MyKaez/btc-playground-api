using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Commands;

public static class SendMessage
{
    public record Command(Guid SessionId, Guid SenderId, Guid ControlId, string Text) : Request<Session>;


    public class Handler : RequestHandler<Command, Session>
    {
        private readonly ISessionService _sessionService;
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;

        public Handler(ISessionService sessionService, IUserService userService, IMessageService messageService)
        {
            _sessionService = sessionService;
            _userService = userService;
            _messageService = messageService;
        }

        public override async Task<Result<Session>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var session = await _sessionService.GetById(request.SessionId,cancellationToken);
            var message = new Message
            {
                SenderId = request.SenderId, Text = request.Text
            };

            if (session is null)
                return NotFound();

            if (request.SessionId == request.SenderId)
            {
                if (session.ControlId != request.ControlId)
                    return NotAuthorized();

                await _messageService.SendMessage(session.Id, message, cancellationToken);
            }
            else
            {
                var user = await _userService.GetById(request.SenderId, cancellationToken);

                if (user is null)
                    return NotFound();

                if (user.ControlId != request.ControlId)
                    return NotAuthorized();
                
                await _messageService.SendMessage(session.Id, message, cancellationToken);
            }

            return session;
        }
    }
}