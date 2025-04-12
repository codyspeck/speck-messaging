namespace Speck.Messaging;

public interface IConsumer<in TMessage>
{
    Task ConsumeAsync(TMessage message, CancellationToken cancellationToken);
}
