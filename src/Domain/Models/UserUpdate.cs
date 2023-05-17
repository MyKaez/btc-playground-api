using System.Text.Json;

namespace Domain.Models;

public record UserUpdate
{
    public Guid SessionId { get; init; }
    
    public Guid UserId { get; init; }
    
    public UserStatus Status { get; init; }
    
    public JsonElement Configuration { get; init; }
}