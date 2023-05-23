using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Queries;

public static class GetCurrentPrices
{
    public record Query : Request<Price[]>;

    public class Handler : RequestHandler<Query, Price[]>
    {
        private readonly IPriceService _priceService;

        public Handler(IPriceService priceService)
        {
            _priceService = priceService;
        }

        public override async Task<RequestResult<Price[], IRequestError>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var prices = await _priceService.GetCurrentPrices(cancellationToken);

            if (prices is null)
                return NotFound();

            var items = prices.ToArray();

            return items;
        }
    }
}