using Application.Models;
using Application.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.Models;
using NotFoundResult = Application.Models.NotFoundResult;

namespace Service.Controllers;

[Route("v1/blocks")]
public class BlockController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

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
            blocks => blocks?.Select(b => _mapper.Map<BlockDto>(b)).ToArray() ?? Array.Empty<BlockDto>()
        );
    }

    private IActionResult Result<T>(RequestResult<T> result, Func<T?, object> ok)
    {
        if (result.IsValid)
            return Ok(ok(result.Result));

        if (ReferenceEquals(result.Error, NotFoundResult.Obj))
            return base.NotFound();

        return base.Problem();
    }
}