using Application;
using Application.Installers;
using Infrastructure.PipelineBehaviours;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Installers;

public class MediatRInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration config)
    {
        services.AddMediatR(options =>
        {
            options.AddOpenBehavior(typeof(LogDuration<,>));
            options.RegisterServicesFromAssembly(typeof(IApplicationMarker).Assembly);
        });
    }
}