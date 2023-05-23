using MediatR;

namespace Application.Models;

public record Request<TResponse> : IRequest<Result<TResponse>>;