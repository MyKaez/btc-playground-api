using Application.Installers;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Installers;

public class BlockchainInfoInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<BlockchainInfoService>();

        services.Configure<BlockchainInfoConfig>(config.GetSection("BlockchainInfo"));
        services.AddHttpClient(BlockchainInfoService.HttpClientFactoryName, (serviceProvider, client) =>
        {
            var apiConfig = serviceProvider.GetRequiredService<IOptions<BlockchainInfoConfig>>();

            client.BaseAddress = new Uri(apiConfig.Value.Url);
            client.Timeout = TimeSpan.FromSeconds(10);
        });
    }
}