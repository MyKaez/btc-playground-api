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
        public JsonElement Configuration { get; init; }
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
                Configuration = request.Configuration
            };
            
            session = await _sessionService.UpdateSession(update, cancellationToken);

            if (session is null)
                return NotFound();

            var res = new RequestResult<Session>(session);

            return res;
        }
    }
}