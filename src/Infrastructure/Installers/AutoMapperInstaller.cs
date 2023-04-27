using Application.Installers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Installers;

public class AutoMapperInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration config)
    {
        services.AddAutoMapper(typeof(InfrastructureInstaller).Assembly);
    }
}