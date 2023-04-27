namespace Domain.Models;

public record Block
{
    public string Id { get; init; }

    public long Height { get; init; }

    public DateTime TimeStamp { get; init; }

    public ulong Nonce { get; init; }

    public double Difficulty { get; init; }
}