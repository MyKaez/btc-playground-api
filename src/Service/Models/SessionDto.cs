﻿using System.Text.Json;

namespace Service.Models;

public record SessionDto
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required JsonElement Configuration { get; init; }
    
    public required DateTime ExpirationTime { get; init; }

    public required IReadOnlyCollection<UserDto> Users { get; init; }
}