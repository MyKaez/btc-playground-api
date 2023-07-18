namespace Infrastructure.Models;

public record Connection
{
    public string ConnectionId { get; init; } = "";

    public Guid SessionId { get; set; }
    
    public Guid? UserId { get; set; }
}