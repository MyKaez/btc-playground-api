using System.Text.Json;

namespace Service.Integration.Models;

public class UserUpdate
{
    public Guid UserId { get; set; }
    
    public string Status { get; set; } = "";

    public JsonElement Configuration { get; set; }
}