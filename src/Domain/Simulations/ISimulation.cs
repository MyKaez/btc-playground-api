using System.Text.Json;

namespace Domain.Simulations;

public interface ISimulation
{
    string SimulationType { get; }
    
    JsonElement? Result { get; set; }
}