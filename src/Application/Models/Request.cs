using MediatR;

namespace Application.Models;

public record Request<TResponse> : IRequest<RequestResult<TResponse, IRequestError>>;