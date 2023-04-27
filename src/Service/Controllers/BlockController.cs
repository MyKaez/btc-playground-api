using Application.Models;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotFoundResult = Application.Models.NotFoundResult;

namespace Service.Controllers;

[Route("v1/blocks")]
public class BlockController : Controller
{
    private readonly IMediator _mediator;

    public BlockController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var query = new GetLatestBlocks.Query();
        var res = await _mediator.Send(query);

        return Result(res);
    }

    private IActionResult Result(RequestResult result)
    {
        if (result.IsValid)
            return Ok(result.Value);

        if (ReferenceEquals(result.Error, NotFoundResult.Obj))
            return base.NotFound();

        return base.Problem();
    }
}