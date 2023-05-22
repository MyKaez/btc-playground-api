using System.Text.Json;
using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;
using Domain.Simulations;

namespace Application.Commands;

public static class ExecuteUserAction
{
    public record Command(Guid SessionId, Guid UserId, Guid UserControlId) : Request<User>
    {
        public UserStatus Status { get; init; }
        
        public JsonElement Configuration { get; init; }
    }

    public class Handler : RequestHandler<Command, User>
    {
        private readonly ISessionService _sessionService;
        private readonly IUserService _userService;

        public Handler(ISessionService sessionService, IUserService userService)
        {
            _sessionService = sessionService;
            _userService = userService;
        }

        public override async Task<RequestResult<User>> Handle(Command request, CancellationToken cancellationToken)
        {
            var session = await _sessionService.GetById(request.SessionId, cancellationToken);

            if (session is null)
                return NotFound();

            var user = await _userService.GetById(request.UserId, cancellationToken);

            if (user is null)
                return NotFound();

            if (user.ControlId != request.UserControlId)
                return NotAuthorized();

            var update = new UserUpdate
            {
                SessionId = session.Id,
                UserId = user.Id,
                Status = request.Status,
                Configuration = request.Configuration
            };

            user = await _userService.Update(update, cancellationToken);

            if (request.Status == UserStatus.Done)
            {
                var end = request.Configuration.Deserialize<ProofOfWorkEnd>(Defaults.Options);

                if (end is null)
                    throw new NotSupportedException();
                
                var sessionUpdate = new SessionUpdate
                {
                    SessionId = session.Id,
                    Action = SessionAction.Stop,
                    Configuration = request.Configuration
                };
                
                await _sessionService.UpdateSession(sessionUpdate, cancellationToken);
            }

            var res = new RequestResult<User>(user!);

            return res;
        }
    }
}