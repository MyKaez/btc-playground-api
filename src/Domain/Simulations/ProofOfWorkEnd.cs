namespace Domain.Simulations;

public class ProofOfWorkEnd : ISimulationEnd
{
    public Guid UserId { get; set; }
    public string Text { get; set; }
    public string Hash { get; set; }
}