using System.Text.Json;

namespace Domain.Models;

public record Session
{
    private readonly List<User> _users = new();

    public required Guid Id { get; init; }

    public Guid? ControlId { get; init; }

    public required string Name { get; init; }

    public SessionStatus Status { get; init; }

    public DateTime ExpiresAt { get; init; }

    public JsonElement? Configuration { get; init; }

    public IEnumerable<User> Users => _users;
    
    public DateTime? StartTime { get; set; }
    
    public DateTime? EndTime { get; set; }

    public Session Add(User user)
    {
        _users.Add(user);
        return this;
    }
}