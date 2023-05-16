﻿using System.Text.Json;

namespace Service.Models.Requests;

public class UserActionRequest
{
    public Guid ControlId { get; init; }
    
    public JsonElement? Data { get; init; }
}