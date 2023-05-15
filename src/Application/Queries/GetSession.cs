using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Queries;

public static class GetSession
{
    public record Query(Guid Id) : Request<Session>;

    public class Handler : RequestHandler<Query, Session>
    {
        private readonly ISessionService _sessionService;

        public Handler(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public override async Task<RequestResult<Session>> Handle(Query request, CancellationToken cancellationToken)
        {
            var session = await _sessionService.GetById(request.Id, cancellationToken);

            if (session is null)
                return NotFound();

            var res = new RequestResult<Session>(session);

            return res;
        }
    }
}