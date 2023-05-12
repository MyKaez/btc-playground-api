using Domain.Models;

namespace Application.Services;

public interface ISuggestionService
{
    SessionSuggestion GetSessionSuggestion();
    
    UserSuggestion GetUserSuggestion();
}