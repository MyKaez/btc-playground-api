using Application.Services;
using Domain.Models;

namespace Infrastructure.Services;

public class BlockService : IBlockService
{
    private readonly MempoolService _mempoolService;

    public BlockService(MempoolService mempoolService)
    {
        _mempoolService = mempoolService;
    }

    public async Task<IEnumerable<Block>?> GetBlocks(CancellationToken cancellationToken)
    {
        var blocks = await _mempoolService.GetBlocks();
        
        return blocks;
    }
}