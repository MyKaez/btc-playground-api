using System.Text.Json;
using Application.Services;
using AutoMapper;
using Domain.Models;
using Infrastructure.Hubs;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using Session = Domain.Models.Session;

namespace Infrastructure.Services;

public class SessionService : ISessionService
{
    private static readonly Dictionary<SessionAction, SessionStatus> ActionStatusMap = new()
    {
        { SessionAction.Prepare, SessionStatus.Preparing },
        { SessionAction.Start, SessionStatus.Started },
        { SessionAction.Stop, SessionStatus.Stopped },
        { SessionAction.Reset, SessionStatus.NotStarted },
    };

    private readonly ISessionRepository _sessionRepository;
    private readonly IMapper _mapper;
    private readonly IHubContext<SessionHub> _hubContext;

    public SessionService(ISessionRepository sessionRepository, IMapper mapper, IHubContext<SessionHub> hubContext)
    {
        _sessionRepository = sessionRepository;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task<IEnumerable<Session>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await _sessionRepository.GetAll(cancellationToken);
        var sessions = _mapper.Map<Session[]>(entities);

        return sessions;
    }

    public async Task<Session?> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _sessionRepository.GetById(id, cancellationToken);
        var session = _mapper.Map<Session>(entity);

        return session;
    }

    public async Task<Session?> CreateSession(
        string name, JsonElement? configuration, CancellationToken cancellationToken)
    {
        var session = new Database.Session
        {
            Id = Guid.NewGuid(),
            ControlId = Guid.NewGuid(),
            Status = SessionStatus.NotStarted.ToString(),
            Name = name,
            Configuration = configuration?.ToString(),
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
        var res = _mapper.Map<Session>(session);

        await _sessionRepository.Add(session, cancellationToken);
        await _hubContext.Clients.All.SendAsync(
            res.Id + ":CreateSession", new { res.Id, res.Status }, cancellationToken
        );

        return res;
    }
    
    public async Task<Session?> UpdateSession(SessionUpdate update, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.Update(
            update.SessionId, session =>
            {
                if (update.Action.HasValue && update.Action != SessionAction.Update)
                    session.Status = ActionStatusMap[update.Action.Value].ToString();

                switch (update.Action)
                {
                    case SessionAction.Start:
                        session.StartTime = DateTime.UtcNow;
                        break;
                    case SessionAction.Stop:
                        session.EndTime = DateTime.UtcNow;
                        break;
                    case SessionAction.Reset:
                        session.StartTime = null;
                        session.EndTime = null;
                        break;
                }

                session.Updated = DateTime.UtcNow;
                session.ExpiresAt = DateTime.UtcNow.AddMinutes(30);
                session.Configuration = update.Configuration.ToString();
            }, cancellationToken);

        if (session is null)
            return null;

        var res = _mapper.Map<Session>(session);

        await _hubContext.Clients.All.SendAsync(update.SessionId + ":SessionUpdate",
            new { res.Id, res.Status, res.Configuration, session.StartTime, session.EndTime }, cancellationToken);

        return res;
    }

    public async Task DeleteUser(Guid sessionId, Guid userId, CancellationToken cancellationToken)
    {
        await _sessionRepository.Update(
            sessionId, session =>
            {
                session.Updated = DateTime.UtcNow;
                session.ExpiresAt = DateTime.UtcNow.AddMinutes(30);

                var interaction = session.Interactions.FirstOrDefault(i => i.UserId == userId);

                if (interaction is not null)
                    interaction.IsDeleted = true;
            }, cancellationToken);

        await _hubContext.Clients.All.SendAsync(sessionId + ":DeleteUser", userId, cancellationToken);
    }

    public async Task DeleteSession(Guid sessionId, CancellationToken cancellationToken)
    {
        await _sessionRepository.Delete(sessionId, cancellationToken);
        await _hubContext.Clients.All.SendAsync(sessionId + ":DeleteSession", sessionId, cancellationToken);
    }
}