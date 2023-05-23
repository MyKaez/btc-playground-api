using System.Diagnostics;
using System.Text.Json;
using Application.Handlers;
using Application.Models;
using Application.Serialization;
using Application.Services;
using Application.Simulations;
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
        private readonly ISimulatorFactory _simulatorFactory;

        public Handler(ISessionService sessionService, IUserService userService, ISimulatorFactory simulatorFactory)
        {
            _sessionService = sessionService;
            _userService = userService;
            _simulatorFactory = simulatorFactory;
        }

        public override async Task<Result<User>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var session = await _sessionService.GetById(request.SessionId, cancellationToken);

            if (session is null)
                return NotFound();

            var user = await _userService.GetById(request.UserId, cancellationToken);

            if (user is null)
                return NotFound();

            if (user.ControlId != request.UserControlId)
                return NotAuthorized();

            var config = request.Configuration;
            var simulation = session.Configuration?.FromJsonElement<Simulation>();
            var simulationType = simulation?.SimulationType ?? "";

            if (simulationType != "")
            {
                var simulator = _simulatorFactory.Create(simulationType);
                var simResult = request.Status switch
                {
                    UserStatus.NotReady => await simulator.UserNotReady(session, config, cancellationToken),
                    UserStatus.Ready => await simulator.UserReady(session, user, config, cancellationToken),
                    UserStatus.Done => await simulator.UserDone(session, config, cancellationToken),
                    _ => throw new UnreachableException()
                };

                if (simResult.TryGetValue(out var conf, out var error))
                    config = conf;
                else
                    return new Result<User>(error);
            }

            user = await UpdateUser(request, config, cancellationToken);

            return user;
        }

        private async Task<User> UpdateUser(Command request, JsonElement config, CancellationToken cancellationToken)
        {
            var update = new UserUpdate
            {
                SessionId = request.SessionId,
                UserId = request.UserId,
                Status = request.Status,
                Configuration = config
            };

            var user = await _userService.Update(update, cancellationToken);

            if (user is null)
                throw new UnreachableException();

            return user;
        }
    }
}