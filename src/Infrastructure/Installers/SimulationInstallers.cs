using Application.Installers;
using Application.Simulations;
using Infrastructure.Simulations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Installers;

public class SimulationInstallers : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration config)
    {
        var simulationTypes = (
            from type in typeof(InfrastructureInstaller).Assembly.GetTypes()
            where type.IsClass && !type.IsAbstract
            where typeof(ISimulator).IsAssignableFrom(type)
            select type
        ).ToArray();

        foreach (var simulationType in simulationTypes)
            services.AddTransient(simulationType);

        services.AddSingleton<ISimulatorFactory>(prov =>
        {
            var dic = simulationTypes.ToDictionary(GetName, s => s);
            var factory = new SimulatorFactory(dic, prov);

            return factory;
        });
    }

    private static string GetName(Type type)
    {
        var fullName = type.FullName!;
        var directory = fullName.Split('.')[^2];
        var res = directory.ToLower()[0] + directory[1..];
        
        return res;
    }
}