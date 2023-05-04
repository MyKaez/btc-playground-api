using System.Reflection;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared;

public static class ServiceProviderFactory
{
    public static IServiceProvider CreateServiceProvider(Action<IServiceCollection, IConfiguration>? servicesFactory = null)
    {
        var (services, config) = CreateInjection();

        servicesFactory?.Invoke(services, config);

        return services.BuildServiceProvider();
    }

    private static (IServiceCollection, IConfiguration) CreateInjection()
    {
        var services = new ServiceCollection();
        var config = CreateConfig();

        InfrastructureInstaller.Install(services, config);

        return (services, config);
    }

    private static IConfigurationRoot CreateConfig()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
            .AddJsonFile("appsettings.Test.json");
        var config = builder.Build();

        return config;
    }
}