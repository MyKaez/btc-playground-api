using Application.Services;
using Bogus;
using Domain.Models;

namespace Infrastructure.Services;

public class SuggestionService : ISuggestionService
{
    private readonly Faker _faker;

    public SuggestionService()
    {
        _faker = new Faker();
    }

    public SessionSuggestion GetSessionSuggestion()
    {
        return new SessionSuggestion(_faker.Commerce.ProductName());
    }

    public UserSuggestion GetUserSuggestion()
    {
        return new UserSuggestion(_faker.Name.FullName());
    }
}