namespace Domain.Models;

public record Message
{
    public Guid SenderId { get; init; }

    public string Text { get; init; } = "";
}