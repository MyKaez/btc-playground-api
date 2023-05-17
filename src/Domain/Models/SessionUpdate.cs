using System.Text.Json;

namespace Domain.Models;

public class SessionUpdate
{
    public Guid SessionId { get; set; }

    public SessionAction Action { get; set; }

    public JsonElement Configuration { get; set; }
}