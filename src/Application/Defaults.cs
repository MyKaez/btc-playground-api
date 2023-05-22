using System.Text.Json;
using Application.Serialization;

namespace Application;

public class Defaults
{
    public static readonly JsonSerializerOptions Options = new JsonSerializerOptions().SetDefaults();
}