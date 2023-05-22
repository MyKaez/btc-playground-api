using System.Text.Json;
using Application.Handlers;
using Application.Models;
using Application.Serialization;
using Application.Services;
using Domain.Models;
using Domain.Simulations;

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
        private readonly IUserService _userService;

        public Handler(ISessionService sessionService, IUserService userService)
        {
            _sessionService = sessionService;
            _userService = userService;
        }

        public override async Task<RequestResult<Session>> Handle(Command request, CancellationToken cancellationToken)
        {
            var session = await _sessionService.GetById(request.SessionId, cancellationToken);

            if (session is null)
                return NotFound();

            if (session.ControlId != request.ControlId)
                return NotAuthorized();

            var config = request.Configuration;
            if (request.Action == SessionAction.Start)
            {
                var users = await _userService.GetBySessionId(session.Id, cancellationToken);
                var configs = users
                    .Select(u => u.Configuration)
                    .Where(u => u.HasValue)
                    .Select(c => c?.Deserialize<ProofOfWorkUser>(Defaults.Options)!)
                    .ToArray();
                var pow = session.Configuration?.Deserialize<ProofOfWork>(Defaults.Options);

                if (pow is null)
                    throw new NotSupportedException();

                ProofOfWork.Calculate(pow, configs.Sum(c => c.HashRate));
                
                config = pow.ToJsonElement();
            }

            var update = new SessionUpdate
            {
                SessionId = session.Id,
                Action = request.Action,
                Configuration = config
            };

            session = await _sessionService.UpdateSession(update, cancellationToken);

            if (session is null)
                return NotFound();

            var res = new RequestResult<Session>(session);

            return res;
        }
    }
}