using System.Text.Json;

namespace Service.Models;

public record SessionDto
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public SessionStatusDto Status { get; init; }
    
    public JsonElement? Configuration { get; init; }
    
    public required DateTime ExpirationTime { get; init; }

    public DateTime? StartTime { get; set; }
    
    public DateTime? EndTime { get; set; }

    public required IReadOnlyCollection<UserDto> Users { get; init; }
}