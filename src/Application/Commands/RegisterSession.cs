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

        public override Task<RequestResult<Session>> Handle(Command request, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var session = _sessionService.CreateService(request.Name, request.Configuration);

                if (session is null)
                    return NotFound();

                var res = new RequestResult<Session>(session);

                return res;
            }, cancellationToken);
        }
    }
}