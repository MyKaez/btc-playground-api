using AutoMapper;
using Infrastructure.Simulations.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.Models;
using Service.Models.Requests;

namespace Service.Controllers;

[Route("v1/simulations")]
public class SimulationController : BaseController
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public SimulationController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("proof-of-work")]
    public IActionResult Post([FromBody] ProofOfWorkRequest request)
    {
        if (!request.TotalHashRate.HasValue || !request.SecondsUntilBlock.HasValue)
            return BadRequest("TotalHashRate and SecondsUntilBlock are required");

        var req = _mapper.Map<ProofOfWorkSession>(request);
        var pow = ProofOfWorkSession.Calculate(req, request.TotalHashRate.Value);
        var res = _mapper.Map<ProofOfWorkDto>(pow);

        return Json(res);
    }
}