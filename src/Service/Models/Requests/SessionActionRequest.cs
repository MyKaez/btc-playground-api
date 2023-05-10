namespace Service.Models.Requests;

public class SessionActionRequest
{
    public Guid ControlId { get; init; }

    public SessionActionDto Action { get; init; }

    public IReadOnlyDictionary<string, object> Data { get; init; } = new Dictionary<string, object>();
}