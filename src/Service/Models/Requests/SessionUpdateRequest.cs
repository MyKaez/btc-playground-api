using System.Text.Json;

namespace Service.Models.Requests;

public class SessionUpdateRequest
{
    public Guid ControlId { get; init; }

    public JsonElement Configuration { get; init; }
}