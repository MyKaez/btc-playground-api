﻿using System.Text.Json;

namespace Domain.Models;

public record Session
{
    private readonly List<User> _users = new();

    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public IEnumerable<User> Users => _users;

    public TimeSpan ExpiresIn { get; init; }

    public required JsonElement Configuration { get; init; }

    public Session Add(User user)
    {
        _users.Add(user);
        return this;
    }
}