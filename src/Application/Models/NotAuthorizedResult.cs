namespace Application.Models;

public record NotAuthorizedResult : IRequestError
{
    private NotAuthorizedResult()
    {
    }

    public static NotAuthorizedResult Obj { get; } = new();
}