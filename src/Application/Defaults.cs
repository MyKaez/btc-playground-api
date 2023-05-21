using System.Text.Json;
using Application.Serialization;

namespace Application;

public class Defaults
{
    public static JsonSerializerOptions Options = new JsonSerializerOptions().SetDefaults();
}