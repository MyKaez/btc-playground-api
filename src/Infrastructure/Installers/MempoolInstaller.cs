using Application.Installers;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Installers;

public class MempoolInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<MempoolService>();

        services.Configure<MempoolConfig>(config.GetSection("Mempool"));
        services.AddHttpClient(MempoolService.HttpClientFactoryName, (serviceProvider, client) =>
        {
            var apiConfig = serviceProvider.GetRequiredService<IOptions<MempoolConfig>>();

            client.BaseAddress = new Uri(apiConfig.Value.Url);
            client.Timeout = TimeSpan.FromSeconds(10);
        });
    }
}