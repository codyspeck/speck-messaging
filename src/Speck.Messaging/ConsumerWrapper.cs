namespace Speck.Messaging;

internal interface IConsumerWrapper
{
    public Task ConsumeAsync(object message, CancellationToken cancellationToken);
}

internal class ConsumerWrapper<TMessage>(IConsumer<TMessage> consumer) : IConsumerWrapper
{
    public Task ConsumeAsync(object message, CancellationToken cancellationToken)
    {
        return consumer.ConsumeAsync((TMessage)message, cancellationToken);
    }
}
