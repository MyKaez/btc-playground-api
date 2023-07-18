using System.Text.Json;
using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Commands;

public static class RegisterSession
{
    public record Command(string Name, JsonElement? Configuration) : Request<Session>;

    public class Handler : RequestHandler<Command, Session>
    {
        private readonly ISessionService _sessionService;

        public Handler(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public override async Task<Result<Session>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var sessions = await _sessionService.GetAll(cancellationToken);

            if (sessions.Any(session => session.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
                return BadRequest("Session name already exists");
            
            var session = await _sessionService.CreateSession(request.Name, request.Configuration, cancellationToken);

            return session ?? NotFound();
        }
    }
}