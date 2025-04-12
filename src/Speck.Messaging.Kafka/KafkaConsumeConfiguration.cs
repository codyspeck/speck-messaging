namespace Speck.Messaging.Kafka;

public class KafkaConsumeConfiguration(string queue)
{
    public string Queue { get; } = queue;
    
    public Type? ExplicitMessageType { get; private set; }

    public KafkaConsumeConfiguration WithExplicitMessageType<TMessage>()
    {
        ExplicitMessageType = typeof(TMessage);

        return this;
    }
}
