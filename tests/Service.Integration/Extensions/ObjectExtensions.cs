using System.Text.Json;
using Service.Serialization;

namespace Service.Integration.Extensions;

public static class ObjectExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions().SetApiDefaults();

    public static JsonElement ToJsonElement(this object obj)
    {
        return JsonSerializer.SerializeToElement(obj, JsonSerializerOptions);
    }
}