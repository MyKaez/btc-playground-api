using Application.Models;
using AutoMapper;
using Infrastructure.Simulations.Models;
using Microsoft.AspNetCore.Mvc;
using Service.Models;
using Service.Models.Requests;

namespace Service.Controllers;

[Route("v1/simulations")]
public class SimulationController : BaseController
{
    private readonly IMapper _mapper;

    public SimulationController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpPost("proof-of-work")]
    public IActionResult Post([FromBody] ProofOfWorkRequest request)
    {
        if (!request.TotalHashRate.HasValue || !request.SecondsUntilBlock.HasValue)
            return BadRequest(new BadRequest("TotalHashRate and SecondsUntilBlock are required"));

        var req = _mapper.Map<ProofOfWorkSession>(request);
        var pow = ProofOfWorkSession.Calculate(req, request.TotalHashRate.Value);
        var res = _mapper.Map<ProofOfWorkDto>(pow);

        return Json(res);
    }
}