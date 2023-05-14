using System.Text.Json;
using System.Text.Json.Nodes;
using Application.Services;
using AutoMapper;
using Domain.Models;
using Infrastructure.Database;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Session = Domain.Models.Session;

namespace Infrastructure.Services;

public class SessionService : ISessionService
{
    private readonly DatabaseContext _databaseContext;
    private readonly IMapper _mapper;
    private readonly IHubContext<SessionHub> _hubContext;

    public SessionService(DatabaseContext databaseContext, IMapper mapper, IHubContext<SessionHub> hubContext)
    {
        _databaseContext = databaseContext;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public IEnumerable<Session> GetAll()
    {
        var entities = _databaseContext.Sessions;
        var sessions = _mapper.Map<Session[]>(entities);

        return sessions;
    }

    public Session? GetById(Guid id)
    {
        var entity = _databaseContext.Find<Database.Session>(id);
        var session = _mapper.Map<Session>(entity);

        return session;
    }

    public async Task<Session?> CreateSession(
        string name, JsonElement? configuration, CancellationToken cancellationToken)
    {
        var entity = new Database.Session
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

        _databaseContext.Sessions.Add(entity);

        var session = _mapper.Map<Session>(entity);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await _hubContext.Clients.All.SendAsync(
            session.Id.ToString(), new { session.Id, session.Status }, cancellationToken
        );

        return session;
    }

    public async Task<Session> StartSession(Session session, CancellationToken cancellationToken)
    {
        var entity = _databaseContext.Find<Database.Session>(session.Id)!;

        entity.Status = SessionStatus.Started.ToString();
        entity.Updated = DateTime.Now;
        entity.ExpiresAt = DateTime.Now.AddMinutes(10);

        _databaseContext.Update(entity);
        
        session = _mapper.Map<Session>(entity);
        
        await _hubContext.Clients.All.SendAsync(
            session.Id.ToString(), new { session.Id, session.Status }, cancellationToken);

        return session;
    }

    public async Task<Session> StopSession(Session session, CancellationToken cancellationToken)
    {
        var entity = _databaseContext.Find<Database.Session>(session.Id)!;

        entity.Status = SessionStatus.Stopped.ToString();
        entity.Updated = DateTime.Now;
        entity.ExpiresAt = DateTime.Now.AddMinutes(10);

        _databaseContext.Update(entity);
        
        session = _mapper.Map<Session>(entity);

        await _hubContext.Clients.All.SendAsync(
            session.Id.ToString(), new { session.Id, session.Status }, cancellationToken);

        return session;
    }

    public async Task<Session> NotifySession(Session session, JsonNode data, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.SendAsync(session.Id.ToString(), data, cancellationToken);

        return session;
    }
}