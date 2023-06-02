﻿using System.Numerics;
using System.Text.Json;
using Domain.Simulations;

namespace Infrastructure.Simulations.Models;

public record ProofOfWorkSession : ISimulation
{
    public static readonly BigInteger Max = BigInteger.Pow(2, 255);

    public string SimulationType => "proofOfWork";

    public int SecondsUntilBlock { get; init; }

    public long? TotalHashRate { get; set; }

    public double? Difficulty { get; set; }

    public double? Expected { get; set; }

    public string? Threshold { get; set; }
    
    public JsonElement? Result { get; set; }

    public static void Calculate(ProofOfWorkSession pow, long totalHashRate)
    {
        pow.TotalHashRate = totalHashRate;

        if (totalHashRate == 0 || pow.SecondsUntilBlock == 0)
        {
            pow.Difficulty = null;
            pow.Expected = null;
            pow.Threshold = null;
        }
        else
        {
            pow.Difficulty = pow.TotalHashRate * pow.SecondsUntilBlock;
            pow.Expected = 1 / pow.Difficulty;
            pow.Threshold = CalculateThreshold(pow.Difficulty!.Value);
        }
    }

    private static string CalculateThreshold(double powDifficulty)
    {
        var difficulty = new BigInteger(powDifficulty);
        var threshold = Max / difficulty;
        var res = threshold.ToString("x").PadLeft(64, '0');

        return res;
    }
}