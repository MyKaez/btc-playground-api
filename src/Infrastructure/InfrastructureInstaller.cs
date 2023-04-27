using Application.Installers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public class InfrastructureInstaller
{
    public static void Install(IServiceCollection services, IConfiguration config)
    {
        var installers =
            from type in typeof(InfrastructureInstaller).Assembly.GetTypes()
            where typeof(IInstaller).IsAssignableFrom(type)
            where type.IsClass && !type.IsAbstract
            select (IInstaller)Activator.CreateInstance(type)!;

        foreach (var installer in installers) 
            installer.Install(services, config);
    }
}