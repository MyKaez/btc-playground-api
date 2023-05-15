using System.Text.Json.Nodes;
using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Commands;

public static class ExecuteUserAction
{
    public record Command(Guid SessionId, Guid UserId, Guid UserControlId, JsonNode Data) : Request<User>;

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

            await _userService.Execute(session, user, request.Data, cancellationToken);

            var res = new RequestResult<User>(user);

            return res;
        }
    }
}