using System.Numerics;
using System.Text.Json;
using Domain.Simulations;

namespace Infrastructure.Simulations.Models;

public record ProofOfWorkSession : ISimulation, ISimulationResult
{
    public static readonly BigInteger Max = BigInteger.Pow(2, 255);

    public string SimulationType => "proofOfWork";

    public int SecondsUntilBlock { get; init; }
    
    public int? SecondsToSkipValidBlocks { get; init; }

    public double? TotalHashRate { get; init; }

    public double? Difficulty { get; init; }

    public double? Expected { get; init; }

    public string? Threshold { get; init; }
    
    public JsonElement? Result { get; init; }

    public static ProofOfWorkSession Calculate(ProofOfWorkSession pow, double totalHashRate)
    {
        pow = pow with { TotalHashRate = totalHashRate };

        if (totalHashRate == 0 || pow.SecondsUntilBlock == 0)
        {
            pow = pow with
            {
                Difficulty = null,
                Expected = null,
                Threshold = null
            };
        }
        else
        {
            var difficulty = totalHashRate * pow.SecondsUntilBlock;
            
            pow = pow with
            {
                Difficulty = difficulty,
                Expected = 1d / difficulty,
                Threshold = CalculateThreshold(difficulty)
            };
        }

        return pow;
    }

    private static string CalculateThreshold(double powDifficulty)
    {
        var difficulty = new BigInteger(powDifficulty);
        var threshold = Max / difficulty;
        var res = threshold.ToString("x").PadLeft(64, '0');

        return res;
    }
}