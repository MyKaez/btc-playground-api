﻿using System.Text.Json;

namespace Service.Models;

public record UserDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = "";
    
    public UserStatusDto Status { get; init; }

    public JsonElement Configuration { get; init; }
}