using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Queries;

public static class GetLatestBlocks
{
    public record Query : Request<Response>;

    public record Response(Block[] Items);

    public class Handler : RequestHandler<Query, Response>
    {
        private readonly IBlockService _blockService;

        public Handler(IBlockService blockService)
        {
            _blockService = blockService;
        }

        public override async Task<RequestResult<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var blocks = await _blockService.GetBlocks(cancellationToken);

            if (blocks is null)
                return NotFound();

            var items = blocks.ToArray();
            var res = new Response(items);
            
            return new RequestResult<Response>(res);
        }
    }
}