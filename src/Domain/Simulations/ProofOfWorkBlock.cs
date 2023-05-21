namespace Domain.Simulations;

public record ProofOfWorkBlock (Guid UserId, string Text, string Hash): ISimulationEnd;