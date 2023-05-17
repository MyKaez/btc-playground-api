namespace Service.Models.Requests;

public record MessageRequest
{
    public Guid ControlId { get; init; }

    public string Text { get; init; } = "";
}