using Domain.Models;

namespace Application.Services;

public interface IPriceService
{
    Task<IReadOnlyCollection<Price>?> GetCurrentPrices(CancellationToken cancellationToken);
}