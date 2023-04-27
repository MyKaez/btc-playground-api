using Domain.Models;

namespace Application.Services;

public interface IBlockService
{
    Task<IEnumerable<Block>?> GetBlocks(CancellationToken cancellationToken);
}