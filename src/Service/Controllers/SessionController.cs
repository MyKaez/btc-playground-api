using Application.Commands;
using Application.Queries;
using AutoMapper;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.Models;
using Service.Models.Requests;

namespace Service.Controllers;

[Route("sessions")]
public class SessionController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SessionController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetSession.Query(id);
        var res = await _mediator.Send(query);

        return Result(res, session => _mapper.Map<SessionDto>(session));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SessionRequest request)
    {
        var cmd = new RegisterSession.Command(request.Name, request.Configuration);
        var res = await _mediator.Send(cmd);

        return Result(res, session => _mapper.Map<SessionControlDto>(session));
    }

    [HttpPost("{sessionId:guid}/actions")]
    public async Task<IActionResult> Post(Guid sessionId, [FromBody] SessionActionRequest request)
    {
        var cmd = new ExecuteSession.Command(sessionId, request.ControlId, _mapper.Map<SessionAction>(request.Action));
        var res = await _mediator.Send(cmd);

        return Result(res, session => _mapper.Map<SessionDto>(session));
    }

    [HttpPost("{sessionId:guid}/users")]
    public async Task<IActionResult> Post(Guid sessionId, [FromBody] UserRequest request)
    {
        var cmd = new RegisterUser.Command(sessionId, request.Name);
        var res = await _mediator.Send(cmd);

        return Result(res, session => _mapper.Map<UserDto>(session));
    }
}