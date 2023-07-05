namespace Service.Models.Requests;

public record ProofOfWorkRequest
{
    public int? TotalHashRate { get; init; }
    
    public int? SecondsUntilBlock { get; init; }
}