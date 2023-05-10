using System.Text.Json;
using System.Text.Json.Serialization;

namespace Service.Integration;

public static class Defaults
{
    static Defaults()
    {
        Options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    public static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}