﻿using System.Text.Json;

namespace Service.Models.Requests;

public class UserActionRequest
{
    public Guid ControlId { get; init; }
    
    public UserStatusDto Status { get; init; }
    
    public JsonElement? Configuration { get; init; }
}