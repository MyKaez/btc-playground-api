using Application.Models;
using MediatR;

namespace Application.Handlers;

public abstract class
    RequestHandler<TInput, TResponse> : IRequestHandler<TInput, RequestResult<TResponse, IRequestError>>
    where TInput : IRequest<RequestResult<TResponse, IRequestError>>
{
    public abstract Task<RequestResult<TResponse, IRequestError>> Handle(TInput request,
        CancellationToken cancellationToken);

    protected RequestResult<TResponse, IRequestError> NotFound()
    {
        return NotFoundResult.Obj;
    }

    protected RequestResult<TResponse, IRequestError> NotAuthorized()
    {
        return NotAuthorizedResult.Obj;
    }

    protected RequestResult<TResponse, IRequestError> BadRequest(string errorMessage)
    {
        return new BadRequest(errorMessage);
    }
}