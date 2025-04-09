using System.Text.Json;
using System.Text.Json.Serialization;

namespace Neptune.Core.Utils;

public static class JsonUtils
{
    private static JsonSerializerOptions? _jsonSerializerOptions;

    public static JsonSerializerOptions? GetDefaultJsonSettings() =>
        _jsonSerializerOptions ??= new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) },
        };
}
