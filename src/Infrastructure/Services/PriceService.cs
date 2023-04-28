using Application.Services;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public class PriceService : IPriceService
{
    private readonly BlockchainInfoService _blockchainInfoService;
    private readonly IMemoryCache _memoryCache;

    public PriceService(BlockchainInfoService blockchainInfoService, IMemoryCache memoryCache)
    {
        _blockchainInfoService = blockchainInfoService;
        _memoryCache = memoryCache;
    }

    public Task<IReadOnlyCollection<Price>?> GetCurrentPrices(CancellationToken cancellationToken)
    {
        return _memoryCache.GetOrCreateAsync(nameof(GetCurrentPrices), entry =>
        {
            entry.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30);
            return _blockchainInfoService.GetPrices(cancellationToken);
        });
    }
}