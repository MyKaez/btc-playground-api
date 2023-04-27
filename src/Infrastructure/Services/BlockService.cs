using Application.Services;
using Domain.Models;

namespace Infrastructure.Services;

public class BlockService : IBlockService
{
    public Task<IEnumerable<Block>?> GetBlocks(CancellationToken cancellationToken)
    {
        return Task.Run(new Func<IEnumerable<Block>?>(() => null), cancellationToken);
    }
}