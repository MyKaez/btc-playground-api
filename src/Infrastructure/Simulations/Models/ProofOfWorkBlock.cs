namespace Infrastructure.Simulations.Models;

public record ProofOfWorkBlock (Guid UserId, string Text, string Hash): ISimulationEnd;