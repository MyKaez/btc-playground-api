using Application.Services;
using Domain.Models;

namespace Infrastructure.Services;

public class PriceService : IPriceService
{
    private readonly BlockchainInfoService _blockchainInfoService;

    public PriceService(BlockchainInfoService blockchainInfoService)
    {
        _blockchainInfoService = blockchainInfoService;
    }

    public  Task<IReadOnlyCollection<Price>?> GetCurrentPrices(CancellationToken cancellationToken)
    {
        return _blockchainInfoService.GetPrices(cancellationToken);
    }
}