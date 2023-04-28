using Application.Models;
using Application.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.Models;

namespace Service.Controllers;

[Route("v1/prices")]
public class PriceController: Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public PriceController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var query = new GetCurrentPrices.Query();
        var res = await _mediator.Send(query);

        return Result(res, price =>  _mapper.Map<PriceDto[]>(price));
    }

    private IActionResult Result<T>(RequestResult<T> result, Func<T, object> ok)
    {
        if (result.IsValid)
            return Ok(ok(result.Result!));

        if (ReferenceEquals(result.Error, Application.Models.NotFoundResult.Obj))
            return base.NotFound();

        return base.Problem();
    }
}