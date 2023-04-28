using Domain.Models;

namespace Application.Services;

public interface IBlockService
{
    Task<IReadOnlyCollection<Block>?> GetBlocks(CancellationToken cancellationToken);
}