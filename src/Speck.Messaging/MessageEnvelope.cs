namespace Speck.Messaging;

public class MessageEnvelope(string body)
{
    public string Body { get; } = body;

    public Dictionary<string, string> Headers { get; } = [];
}
