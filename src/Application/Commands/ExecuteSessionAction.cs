using System.Text.Json.Nodes;
using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Commands;

public static class ExecuteSessionAction
{
    public record Command(Guid SessionId, Guid ControlId, SessionAction Action) : Request<Session>
    {
        public JsonNode? Data { get; init; }
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
            var session = _sessionService.GetById(request.SessionId);

            if (session is null)
                return NotFound();

            if (session.ControlId != request.ControlId)
                return BadRequest($"The provided control id for the session {request.SessionId} is incorrect.");

            session = request.Action switch
            {
                SessionAction.Start => await _sessionService.StartSession(session, cancellationToken),
                SessionAction.Stop => await _sessionService.StopSession(session, cancellationToken),
                SessionAction.Notify => await _sessionService.NotifySession(session, request.Data!, cancellationToken),
                _ => throw new NotSupportedException($"Cannot handle action '{request.Action}'")
            };

            var res = new RequestResult<Session>(session);

            return res;
        }
    }
}