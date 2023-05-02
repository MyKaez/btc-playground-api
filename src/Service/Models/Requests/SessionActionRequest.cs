namespace Service.Models.Requests;

public class SessionActionRequest
{
    public Guid ControlId { get; init; }

    public SessionAction Action { get; init; }
}