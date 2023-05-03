namespace Infrastructure.Mempool;

public record MempoolHashRate
{
    public double CurrentHashRate { get; init; }
    public double CurrentDifficulty { get; init; }
}