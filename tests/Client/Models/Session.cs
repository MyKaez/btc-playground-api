﻿namespace Client.Models;

public record Session
{
    public Guid Id { get; set; }
    
    public Guid ControlId { get; set; }
}