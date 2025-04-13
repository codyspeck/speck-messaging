namespace Speck.Messaging.Kafka;

public class KafkaConsumeConfiguration(string queue)
{
    public string Queue { get; } = queue;

    public int BoundedCapacity { get; private set; } = 1;
    
    public int MaxDegreeOfParallelism { get; private set; } = 1;

    public bool CreateOnStartup { get; private set; }

    public Type? ExplicitMessageType { get; private set; }

    public KafkaConsumeConfiguration CreateOnStartupIf(bool condition)
    {
        CreateOnStartup = condition;

        return this;
    }
    
    public KafkaConsumeConfiguration WithBoundedCapacity(int boundedCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(boundedCapacity);
        
        BoundedCapacity = boundedCapacity;
        
        return this;
    }

    public KafkaConsumeConfiguration WithMaxDegreeOfParallelism(int maxDegreeOfParallelism)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxDegreeOfParallelism);
        
        MaxDegreeOfParallelism = maxDegreeOfParallelism;

        return this;
    }
    
    public KafkaConsumeConfiguration WithExplicitMessageType<TMessage>()
    {
        ExplicitMessageType = typeof(TMessage);

        return this;
    }
}
