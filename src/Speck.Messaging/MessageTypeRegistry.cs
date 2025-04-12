namespace Speck.Messaging;

internal class MessageTypeRegistry
{
    private readonly Dictionary<string, Type> _messageTypes = [];
    private readonly Dictionary<Type, string> _messageTypeNames = [];

    public void Add<TMessage>(string messageTypeName)
    {
        _messageTypes.Add(messageTypeName, typeof(TMessage));
        _messageTypeNames.Add(typeof(TMessage), messageTypeName);
    }

    public Type GetMessageType(string messageTypeName)
    {
        return _messageTypes[messageTypeName];
    }

    public string GetMessageTypeName(Type type)
    {
        return _messageTypeNames[type];
    }
}
