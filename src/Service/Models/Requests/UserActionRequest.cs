using System.Text.Json.Nodes;

namespace Service.Models.Requests;

public class UserActionRequest
{
    public JsonNode Data { get; init; } = new JsonObject();
}