using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Installers;

public interface IInstaller
{
    void Install(IServiceCollection services, IConfiguration config);
}