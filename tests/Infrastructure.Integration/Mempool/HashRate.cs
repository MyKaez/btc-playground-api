using Infrastructure.Mempool;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Xunit;

namespace Infrastructure.Integration.Mempool;

public class HashRate
{
    private readonly MempoolService _service;

    public HashRate()
    {
        var provider = ServiceProviderFactory.CreateServiceProvider();

        _service = provider.GetRequiredService<MempoolService>();
    }

    [Fact]
    public async Task GetHashRateInfo()
    {
        var info = await _service.GetHashRateInfo(CancellationToken.None);

        Assert.NotNull(info);
    }
}