namespace Application.Models;

public record BadRequest(string ErrorMessage) : IRequestError
{
}