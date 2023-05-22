﻿using System.Diagnostics;
using System.Text.Json;
using Application.Handlers;
using Application.Models;
using Application.Serialization;
using Application.Services;
using Application.Simulations;
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
        private readonly ISimulatorFactory _simulatorFactory;

        public Handler(ISessionService sessionService, ISimulatorFactory simulatorFactory)
        {
            _sessionService = sessionService;
            _simulatorFactory = simulatorFactory;
        }

        public override async Task<RequestResult<Session>> Handle(Command request, CancellationToken cancellationToken)
        {
            var session = await _sessionService.GetById(request.SessionId, cancellationToken);

            if (session is null)
                return NotFound();

            if (session.ControlId != request.ControlId)
                return NotAuthorized();

            var config = request.Configuration;
            var simulationType = config.FromJsonElement<Simulation>()?.SimulationType;

            if (simulationType is null)
                return BadRequest("No SimulationType was given");

            var simulator = _simulatorFactory.Create(simulationType);
            var simResult = request.Action switch
            {
                SessionAction.Prepare => await simulator.SessionPrepare(session, config, cancellationToken),
                SessionAction.Start => await simulator.SessionStart(session, config, cancellationToken),
                SessionAction.Stop => await simulator.SessionStop(session, config, cancellationToken),
                SessionAction.Reset => await simulator.SessionReset(session, config, cancellationToken),
                _ => throw new UnreachableException()
            };

            if (simResult is not null)
            {
                if (simResult.IsValid)
                    config = simResult.Result;
                else
                    return new RequestResult<Session>(simResult.Error!);
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