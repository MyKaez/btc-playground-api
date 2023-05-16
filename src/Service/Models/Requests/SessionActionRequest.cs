using System.Text.Json;

namespace Service.Models.Requests;

public class SessionActionRequest
{
    public Guid ControlId { get; init; }

    public SessionActionDto Action { get; init; }

    public JsonElement? Data { get; init; }
}