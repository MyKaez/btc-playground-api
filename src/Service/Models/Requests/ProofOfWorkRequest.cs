namespace Service.Models.Requests;

public record ProofOfWorkRequest
{
    public double? TotalHashRate { get; init; }
    
    public int? SecondsUntilBlock { get; init; }
}