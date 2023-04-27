namespace Application.Models;

public record NotFoundResult : IRequestError
{
    private NotFoundResult()
    {
    }

    public static NotFoundResult Obj { get; } = new();
}