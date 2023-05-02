using Application.Commands;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Xunit;

namespace Infrastructure.Integration;

public class Sessions
{
    private readonly IMediator _mediator;

    public Sessions()
    {
        var provider = ServiceProviderFactory.CreateServiceProvider();

        _mediator = provider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task StartAndStopSession()
    {
        var session = await CreateSession();
        var start = await StartSession(session.SessionId, session.ControlId);
        var stop = await StopSession(session.SessionId, session.ControlId);
        
        Assert.NotEqual(start, stop);
    }

    private async Task<(Guid SessionId, Guid ControlId)> CreateSession()
    {
        var command = new RegisterSession.Command("test-session", null);
        var result = await _mediator.Send(command);
        var session = result.Result;

        Assert.NotNull(session);
        Assert.NotNull(session.ControlId);
        Assert.Equal(SessionExecutionStatus.NotStarted, session.ExecutionStatus);

        return (session.Id, session.ControlId.Value);
    }

    private async Task<SessionExecutionStatus> StartSession(Guid sessionId, Guid controlId)
    {
        var command = new ExecuteSession.Command(sessionId, controlId, SessionAction.Start);
        var res = await _mediator.Send(command);

        Assert.NotNull(res.Result);
        Assert.Equal(SessionExecutionStatus.Started, res.Result.ExecutionStatus);

        return res.Result.ExecutionStatus;
    }

    private async Task<object> StopSession(Guid sessionId, Guid controlId)
    {
        var command = new ExecuteSession.Command(sessionId, controlId, SessionAction.Stop);
        var res = await _mediator.Send(command);

        Assert.NotNull(res.Result);
        Assert.Equal(SessionExecutionStatus.Stopped, res.Result.ExecutionStatus);

        return res.Result.ExecutionStatus;
    }
}