using Application;
using Application.Installers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Installers;

public class MediatRInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration config)
    {
        services.AddMediatR(options => options.RegisterServicesFromAssembly(typeof(IApplicationMarker).Assembly));
    }
}