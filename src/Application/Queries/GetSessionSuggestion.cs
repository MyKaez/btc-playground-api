using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Queries;

public static class GetSessionSuggestion
{
    public record Query : Request<SessionSuggestion>;

    public class Handler : RequestHandler<Query, SessionSuggestion>
    {
        private readonly ISuggestionService _suggestionService;

        public Handler(ISuggestionService suggestionService)
        {
            _suggestionService = suggestionService;
        }

        public override Task<RequestResult<SessionSuggestion, IRequestError>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var suggestion = _suggestionService.GetSessionSuggestion();

            return Task.FromResult<RequestResult<SessionSuggestion, IRequestError>>(suggestion);
        }
    }
}