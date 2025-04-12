namespace Speck.Messaging.Kafka;

public class KafkaConsumeConfiguration(string queue)
{
    public string Queue { get; } = queue;
}
