namespace Speck.Messaging;

public interface IBatchConsumer<in TMessage>
{
    Task ConsumeAsync(TMessage[] messages);
}
