﻿using Infrastructure.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public class ConnectionService : IConnectionService
{
    private readonly IMemoryCache _memoryCache;

    public ConnectionService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void Add(string connectionId, Guid sessionId)
    {
        var connections = _memoryCache.GetOrCreate<List<Connection>>(
            "Connections",
            _ => new List<Connection>()
        );
        var con = new Connection { ConnectionId = connectionId, SessionId = sessionId};
        
        connections!.Add(con);
    }

    public void Update(string connectionId, Guid userId)
    {
        var connection = Get(connectionId);

        if (connection is not null)
            connection.UserId = userId;
    }

    public void Remove(string connectionId)
    {
        var connections = _memoryCache.Get<List<Connection>>("Connections")!;
        var con = connections!.First(c => c.ConnectionId == connectionId);

        connections.Remove(con);
    }

    public Connection? Get(string connectionId)
    {
        var connections = _memoryCache.Get<List<Connection>>("Connections")!;
        var con = connections?.FirstOrDefault(c => c.ConnectionId == connectionId);

        return con;
    }
}