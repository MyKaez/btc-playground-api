﻿using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Queries;

public static class GetSessions
{
    public record Query : Request<Session[]>;

    public class Handler : RequestHandler<Query, Session[]>
    {
        private readonly ISessionService _sessionService;

        public Handler(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public override async Task<Result<Session[]>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var sessions = await _sessionService.GetAll(cancellationToken);

            return sessions.ToArray();
        }
    }
}