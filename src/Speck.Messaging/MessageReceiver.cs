namespace Speck.Messaging;

internal class MessageReceiver(
    MessageTypeRegistry messageTypeRegistry,
    IMessageSerializer messageSerializer,
    Func<Type, IConsumePipeline> consumePipelineFactory)
{
    public async Task ReceiveAsync(MessageEnvelope envelope)
    {
        var messageType = messageTypeRegistry.GetMessageType(envelope.Headers[MessageHeaders.MessageType]);

        var message = messageSerializer.Deserialize(envelope.Body, messageType);
        
        await consumePipelineFactory
            .Invoke(messageType)
            .SendAsync(message);
    }    
}
