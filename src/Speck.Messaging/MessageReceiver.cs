namespace Speck.Messaging;

internal class MessageReceiver(
    MessageTypeRegistry messageTypeRegistry,
    IMessageSerializer messageSerializer,
    Func<Type, IConsumerWrapper> consumerWrapperFactory)
{
    public async Task ReceiveAsync(MessageEnvelope envelope, CancellationToken cancellationToken)
    {
        var messageType = messageTypeRegistry.GetMessageType(envelope.Headers[MessageHeaders.MessageType]);

        var message = messageSerializer.Deserialize(envelope.Body, messageType);
        
        await consumerWrapperFactory
            .Invoke(messageType)
            .ConsumeAsync(message, cancellationToken);
    }    
}
