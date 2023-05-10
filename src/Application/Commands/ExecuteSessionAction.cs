using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Commands;

public static class ExecuteSessionAction
{
    public record Command(Guid SessionId, Guid ControlId, SessionAction Action) : Request<Session>;

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

            if (request.Action == SessionAction.Start)
                session = await _sessionService.StartSession(session, cancellationToken);
            else if (request.Action == SessionAction.Stop)
                session = await _sessionService.StopSession(session, cancellationToken);

            var res = new RequestResult<Session>(session);

            return res;
        }
    }
}