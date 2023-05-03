namespace Infrastructure.Mempool;

public record MempoolBlock
{
    public string Id { get; init; } = null!;

    public long Height { get; init; }

    public long TimeStamp { get; init; }

    public ulong Nonce { get; init; }

    public double Difficulty { get; init; }
}