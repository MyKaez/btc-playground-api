using System.Text.Json.Nodes;

namespace Service.Models.Requests;

public class UserActionRequest
{
    public Guid ControlId { get; init; }
    
    public JsonNode Data { get; init; } = new JsonObject();
}