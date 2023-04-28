using Application.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.Models;

namespace Service.Controllers;

[Route("v1/prices")]
public class PriceController : BaseController
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

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

        return Result(res,
            price => _mapper.Map<PriceDto[]>(price)
        );
    }
}