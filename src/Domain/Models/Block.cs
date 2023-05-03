namespace Domain.Models;

public record Block
{
    public string Id { get; init; } = null!;

    public long Height { get; init; }

    public DateTime TimeStamp { get; init; }

    public ulong Nonce { get; init; }

    public double Difficulty { get; init; }

    public double Probability => 1 / Difficulty / Defaults.BlockDurationInSeconds;
}