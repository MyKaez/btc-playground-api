using System.Text.Json;
using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Commands;

public static class ExecuteSessionAction
{
    public record Command(Guid SessionId, Guid ControlId, SessionAction Action) : Request<Session>
    {
        public JsonElement Data { get; init; }
    }

    public class Handler : RequestHandler<Command, Session>
    {
        private readonly ISessionService _sessionService;

        public Handler(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public override async Task<RequestResult<Session>> Handle(Command request, CancellationToken cancellationToken)
        {
            var session = await _sessionService.GetById(request.SessionId, cancellationToken);

            if (session is null)
                return NotFound();

            if (session.ControlId != request.ControlId)
                return NotAuthorized();

            var update = new SessionUpdate
            {
                SessionId = session.Id,
                Action = request.Action,
                Data = request.Data
            };
            session = request.Action switch
            {
                SessionAction.Prepare => await _sessionService.UpdateSession(update, cancellationToken),
                SessionAction.Start => await _sessionService.UpdateSession(update, cancellationToken),
                SessionAction.Stop => await _sessionService.UpdateSession(update, cancellationToken),
                SessionAction.Notify => await _sessionService.NotifySession(session.Id, request.Data!, cancellationToken),
                _ => throw new NotSupportedException($"Cannot handle action '{request.Action}'")
            };

            if (session is null)
                return NotFound();

            var res = new RequestResult<Session>(session);

            return res;
        }
    }
}