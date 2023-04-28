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

    public Task<IReadOnlyCollection<Block>?> GetBlocks(CancellationToken cancellationToken)
    {
        return _mempoolService.GetBlocks(cancellationToken);
    }
}