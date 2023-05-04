using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Commands;

public static class RegisterUser
{
    public record Command(Guid SessionId, string UserName) : Request<User>;

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
            var session = _sessionService.GetById(request.SessionId);

            if (session is null)
                return NotFound();

            var user = await _userService.Create(session, request.UserName, cancellationToken);
            var res = new RequestResult<User>(user);

            return res;
        }
    }
}