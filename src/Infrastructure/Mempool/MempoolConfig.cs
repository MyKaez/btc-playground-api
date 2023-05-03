namespace Infrastructure.Mempool;

public record MempoolConfig
{
    public string Url { get; init; } = "";
}