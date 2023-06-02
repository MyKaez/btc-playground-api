using System.Text.Json;

namespace Infrastructure.Simulations.Models;

public interface ISimulationResult
{
    JsonElement? Result { get; }
}