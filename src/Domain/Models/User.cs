﻿namespace Domain.Models;

public record User
{
    public Guid Id { get; init; }

    public Guid ControlId { get; init; }

    public string Name { get; init; } = "";

    public UserStatus Status { get; init; }
}