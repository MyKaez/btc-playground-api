using Application.Handlers;
using Application.Models;
using Application.Services;
using Domain.Models;

namespace Application.Queries;

public static class GetUserSuggestion
{
    public record Query : Request<UserSuggestion>;

    public class Handler : RequestHandler<Query, UserSuggestion>
    {
        private readonly ISuggestionService _suggestionService;

        public Handler(ISuggestionService suggestionService)
        {
            _suggestionService = suggestionService;
        }

        public override Task<RequestResult<UserSuggestion, IRequestError>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var suggestion = _suggestionService.GetUserSuggestion();

            return Task.FromResult<RequestResult<UserSuggestion, IRequestError>>(suggestion);
        }
    }
}