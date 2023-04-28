using Application.Installers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Infrastructure.Installers;

public class SerilogInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration config)
    {
        services.AddLogging(
            loggingBuilder =>
            {
                var logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();

                loggingBuilder.AddSerilog(logger, true);
            });
    }
}