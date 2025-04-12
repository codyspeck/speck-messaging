namespace Speck.Messaging;

internal class MessageSender(
    IMessageSerializer messageSerializer,
    MessageTypeRegistry messageTypeRegistry,
    Func<Type, IEndpoint> endpointFactory) : IMessageSender
{
    public async Task SendAsync(object message)
    {
        var messageType = message.GetType();
        
        var messageTypeName = messageTypeRegistry.GetMessageTypeName(messageType);

        var messageEnvelope = new MessageEnvelope(messageSerializer.Serialize(message));
        
        messageEnvelope.Headers.Add(MessageHeaders.MessageType, messageTypeName);
        
        await endpointFactory
            .Invoke(messageType)
            .SendAsync(messageEnvelope);
    }
}
