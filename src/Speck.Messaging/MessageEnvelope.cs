namespace Speck.Messaging;

public class MessageEnvelope(string body, Type? explicitMessageType = null)
{
    public string Body { get; } = body;

    public Type? ExplicitMessageType { get; } = explicitMessageType;

    public Dictionary<string, string> Headers { get; } = [];
}
