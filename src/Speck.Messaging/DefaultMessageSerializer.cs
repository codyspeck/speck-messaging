using System.Text.Json;

namespace Speck.Messaging;

public class DefaultMessageSerializer : IMessageSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public object Deserialize(string message, Type type)
    {
        return JsonSerializer.Deserialize(message, type, Options)!;
    }

    public string Serialize(object message)
    {
        return JsonSerializer.Serialize(message, Options);
    }
}