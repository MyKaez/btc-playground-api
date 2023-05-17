﻿using System.Text.Json;
using Domain.Models;

namespace Application.Services;

public interface IUserService
{
    Task<User?> GetById(Guid userId, CancellationToken cancellationToken);
    
    Task<User> Create(Session session, string userName, CancellationToken cancellationToken);

    Task Execute(Guid sessionId, Guid userId, JsonElement configuration, CancellationToken cancellationToken);
}