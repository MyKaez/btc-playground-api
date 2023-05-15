using System.Text.Json;
using System.Text.Json.Nodes;
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
    private readonly ISessionRepository _sessionRepository;
    private readonly IMapper _mapper;
    private readonly IHubContext<SessionHub> _hubContext;

    public SessionService(ISessionRepository sessionRepository, IMapper mapper, IHubContext<SessionHub> hubContext)
    {
        _sessionRepository = sessionRepository;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public IEnumerable<Session> GetAll(CancellationToken cancellationToken)
    {
        var entities = _sessionRepository.GetAll(cancellationToken);
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
            Created = DateTime.Now,
            Updated = DateTime.Now,
            ExpiresAt = DateTime.Now.AddMinutes(10)
        };
        var res = _mapper.Map<Session>(session);

        await _sessionRepository.Add(session, cancellationToken);
        await _hubContext.Clients.All.SendAsync(
            res.Id.ToString(), new { res.Id, res.Status }, cancellationToken
        );

        return res;
    }

    public async Task<Session?> StartSession(Guid sessionId, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.Update(sessionId, entity =>
        {
            entity.Status = SessionStatus.Started.ToString();
            entity.Updated = DateTime.Now;
            entity.ExpiresAt = DateTime.Now.AddMinutes(10);
        }, cancellationToken);
        
        if (session is null)
            return null;

        var res = _mapper.Map<Session>(session);

        await _hubContext.Clients.All.SendAsync(
            sessionId.ToString(), new { res.Id, res.Status }, cancellationToken);

        return res;
    }

    public async Task<Session?> StopSession(Guid sessionId, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.Update(sessionId, entity =>
        {
            entity.Status = SessionStatus.Stopped.ToString();
            entity.Updated = DateTime.Now;
            entity.ExpiresAt = DateTime.Now.AddMinutes(10);
        }, cancellationToken);

        if (session is null)
            return null;
        
        var res = _mapper.Map<Session>(session);

        await _hubContext.Clients.All.SendAsync(
            sessionId.ToString(), new { res.Id, res.Status }, cancellationToken);

        return res;
    }

    public async Task<Session?> NotifySession(Guid sessionId, JsonNode data, CancellationToken cancellationToken)
    {
        var session = await GetById(sessionId, cancellationToken);

        if (session is null)
            return null;
        
        await _hubContext.Clients.All.SendAsync(sessionId.ToString(), data, cancellationToken);

        return session;
    }
}