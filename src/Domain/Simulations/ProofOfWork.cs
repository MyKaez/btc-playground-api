namespace Domain.Simulations;

public class ProofOfWork : ISimulation
{
    public string SimulationType => "proofOfWork";

    public int SecondsUntilBlock { get; set; }

    public long? TotalHashRate { get; set; }

    public double? Difficulty { get; set; }
    
    public double? Expected { get; set; }
}