using Application.Services;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public class BlockService : IBlockService
{
    private readonly MempoolService _mempoolService;
    private readonly IMemoryCache _memoryCache;

    public BlockService(MempoolService mempoolService, IMemoryCache memoryCache)
    {
        _mempoolService = mempoolService;
        _memoryCache = memoryCache;
    }

    public Task<IReadOnlyCollection<Block>?> GetBlocks(CancellationToken cancellationToken)
    {
        return _memoryCache.GetOrCreateAsync(nameof(GetBlocks), entry =>
        {
            entry.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30);

            return _mempoolService.GetBlocks(cancellationToken);
        });
    }
}