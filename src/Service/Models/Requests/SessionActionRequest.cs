using System.Text.Json.Nodes;

namespace Service.Models.Requests;

public class SessionActionRequest
{
    public Guid ControlId { get; init; }

    public SessionActionDto Action { get; init; }

    public JsonNode Data { get; init; } = new JsonObject();
}