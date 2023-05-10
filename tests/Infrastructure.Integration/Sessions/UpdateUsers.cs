using Application.Commands;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Infrastructure.Integration.Sessions;

public class UpdateUsers
{
    private const string UserName = "kenny";

    private readonly IMediator _mediator;

    public UpdateUsers()
    {
        var serviceProvider = Shared.ServiceProviderFactory.CreateServiceProvider();

        _mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task UpdateUsersInSession()
    {
        var session = await CreateSession();
        var user = await CreateUser(session.Id);
        
    }

    private async Task<Session> CreateSession()
    {
        var req = new RegisterSession.Command("test-session", null);
        var res = await _mediator.Send(req);
        var session = res.Result!;

        return session;
    }

    private async Task<User> CreateUser(Guid sessionId)
    {
        var req = new RegisterUser.Command(sessionId, UserName);
        var res = await _mediator.Send(req);
        var user = res.Result!;

        return user;
    }
}