﻿using System.Text.Json.Nodes;
using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Commands;

public static class ExecuteUserAction
{
    public record Command(Guid SessionId, Guid UserId, JsonNode Data) : Request<User>;

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

            var user = _userService.GetById(session, request.UserId);

            if (user is null)
                return NotFound();

            await _userService.Execute(session, user, request.Data, cancellationToken);

            var res = new RequestResult<User>(user);

            return res;
        }
    }
}