using Application.Models;
using MediatR;

namespace Application.Handlers;

public abstract class RequestHandler<TInput, TResponse> : IRequestHandler<TInput, Result<TResponse>>
    where TInput : IRequest<Result<TResponse>>
{
    public abstract Task<Result<TResponse>> Handle(TInput request,
        CancellationToken cancellationToken);

    protected Result<TResponse> NotFound()
    {
        return NotFoundResult.Obj;
    }

    protected Result<TResponse> NotAuthorized()
    {
        return NotAuthorizedResult.Obj;
    }

    protected Result<TResponse> BadRequest(string errorMessage)
    {
        return new BadRequest(errorMessage);
    }
}