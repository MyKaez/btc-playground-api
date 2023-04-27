using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Queries;

public static class GetLatestBlocks
{
    public record Query : Request<Block[]>;

    public class Handler : RequestHandler<Query, Block[]>
    {
        private readonly IBlockService _blockService;

        public Handler(IBlockService blockService)
        {
            _blockService = blockService;
        }

        public override async Task<RequestResult<Block[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var blocks = await _blockService.GetBlocks(cancellationToken);

            if (blocks is null)
                return NotFound();

            var items = blocks.ToArray();

            return new RequestResult<Block[]>(items);
        }
    }
}