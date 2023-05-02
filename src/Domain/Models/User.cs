namespace Domain.Models;

public record User
{
    public Guid Id { get; init; }

    public string Name { get; init; } = "";
}