using Application.Models;
using MediatR;

namespace Application.Handlers;

public abstract class RequestHandler<TInput, TResponse> : IRequestHandler<TInput, RequestResult<TResponse>>
    where TInput : IRequest<RequestResult<TResponse>>
{
    public abstract Task<RequestResult<TResponse>> Handle(TInput request, CancellationToken cancellationToken);

    protected RequestResult<TResponse> NotFound()
    {
        return new RequestResult<TResponse>(NotFoundResult.Obj);
    }
    protected RequestResult<TResponse> NotAuthorized()
    {
        return new RequestResult<TResponse>(NotAuthorizedResult.Obj);
    }
    
    protected RequestResult<TResponse> BadRequest(string errorMessage)
    {
        return new RequestResult<TResponse>(new BadRequest(errorMessage));
    }
}