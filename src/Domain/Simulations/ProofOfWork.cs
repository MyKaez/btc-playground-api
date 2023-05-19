namespace Domain.Simulations;

public class ProofOfWork : ISimulation
{
    public string SimulationType => "proofOfWork";

    public int SecondsUntilBlock { get; set; }
}