using Application.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.Models;

namespace Service.Controllers;

[Route("v1/blocks")]
public class BlockController : BaseController
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public BlockController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var query = new GetLatestBlocks.Query();
        var res = await _mediator.Send(query);

        return Result(res,
            blocks => _mapper.Map<BlockDto[]>(blocks)
        );
    }
}