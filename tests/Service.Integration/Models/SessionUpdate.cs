using System.Text.Json;

namespace Service.Integration.Models;

public class SessionUpdate
{
    public Guid SessionId { get; set; }

    public string Status { get; set; } = "";

    public JsonElement Configuration { get; set; }
}