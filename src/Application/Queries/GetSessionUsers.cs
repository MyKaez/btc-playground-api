﻿using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Queries;

public static class GetSessionUsers
{
    public record Query(Guid SessionId) : Request<User[]>;

    public class Handler : RequestHandler<Query, User[]>
    {
        private readonly ISessionService _sessionService;
        private readonly IUserService _userService;

        public Handler(ISessionService sessionService, IUserService _userService)
        {
            _sessionService = sessionService;
            this._userService = _userService;
        }
        
        public override async Task<RequestResult<User[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var session = await _sessionService.GetById(request.SessionId, cancellationToken);

            if (session is null)
                return NotFound();

            var users = await _userService.GetBySessionId(session.Id, cancellationToken);

            return new RequestResult<User[]>(users);
        }
    }
}