﻿using Application.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.Models;

namespace Service.Controllers;

public class SuggestionController : BaseController
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public SuggestionController(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpGet("v1/suggestions/sessions")]
    public async Task<IActionResult> GetSession()
    {
        var query = new GetSessionSuggestion.Query();
        var res = await _mediator.Send(query);

        return Result(res, suggestion => _mapper.Map<SessionSuggestionDto>(suggestion));
    }

    [HttpGet("v1/suggestions/users")]
    public async Task<IActionResult> GetUser()
    {
        var query = new GetUserSuggestion.Query();
        var res = await _mediator.Send(query);

        return Result(res, suggestion => _mapper.Map<UserSuggestionDto>(suggestion));
    }
}