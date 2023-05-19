using System.Text.Json;
using System.Text.Json.Serialization;

namespace Service.Serialization;

public static class JsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions SetApiDefaults(this JsonSerializerOptions options)
    {
        var namingPolicy = JsonNamingPolicy.CamelCase;
        
        options.PropertyNamingPolicy = namingPolicy;
        options.Converters.Add(new JsonStringEnumConverter(namingPolicy));

        return options;
    }
}