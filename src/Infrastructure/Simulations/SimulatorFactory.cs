using Application.Simulations;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Simulations;

public class SimulatorFactory : ISimulatorFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IReadOnlyDictionary<string, Type> _simulationTypes;

    public SimulatorFactory(IReadOnlyDictionary<string, Type> simulationTypes, IServiceProvider serviceProvider)
    {
        _simulationTypes = simulationTypes;
        _serviceProvider = serviceProvider;
    }

    public ISimulator Create(string simulationType)
    {
        if (_simulationTypes.TryGetValue(simulationType, out var type))
            return (ISimulator)_serviceProvider.GetRequiredService(type);

        throw new NotSupportedException("Unknown simulation type: " + simulationType);
    }
}