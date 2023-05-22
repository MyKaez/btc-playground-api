using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Serialization;

public static class JsonExtensions
{
    public static JsonElement ToJsonElement(this object obj)
    {
        return JsonSerializer.SerializeToElement(obj, Defaults.Options);
    }
    
    public static T? FromJsonElement<T>(this JsonElement obj)
    {
        return obj.Deserialize<T>(Defaults.Options);
    }

    public static JsonSerializerOptions SetDefaults(this JsonSerializerOptions options)
    {
        var namingPolicy = JsonNamingPolicy.CamelCase;

        options.PropertyNamingPolicy = namingPolicy;
        options.Converters.Add(new JsonStringEnumConverter(namingPolicy));

        return options;
    }
}