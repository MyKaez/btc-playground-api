﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace Client;

public static class Defaults
{
    static Defaults()
    {
        Options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    public static JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}