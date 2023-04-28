using Application.Installers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Installers;

public class CacheInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration config)
    {
        services.AddMemoryCache();
    }
}