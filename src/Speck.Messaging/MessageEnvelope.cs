namespace Speck.Messaging;

public class MessageEnvelope(string body)
{
    public string Body { get; } = body;

    public Type? ExplicitMessageType { get; private set; }

    public Dictionary<string, string> Headers { get; } = [];

    public MessageEnvelope WithExplicitMessageType(Type? explicitMessageType)
    {
        ExplicitMessageType = explicitMessageType;

        return this;
    }
}
