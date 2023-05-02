﻿using System.Text.Json;

namespace Service.Models.Requests;

public record SessionRequest
{
    public required string Name { get; init; }

    public required JsonElement Configuration { get; init; }
}