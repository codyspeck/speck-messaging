namespace Speck.Messaging.Sqs;

public class SqsConsumeConfiguration(string queueName)
{
    public string QueueName { get; } = queueName;

    public Type? ExplicitMessageType { get; private set; }
    
    public SqsConsumeConfiguration WithExplicitMessageType(Type explicitMessageType)
    {
        ExplicitMessageType = explicitMessageType;
        
        return this;
    }
}
