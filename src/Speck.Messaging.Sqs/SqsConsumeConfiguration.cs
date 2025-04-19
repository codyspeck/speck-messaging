namespace Speck.Messaging.Sqs;

public class SqsConsumeConfiguration(string queueUrl)
{
    public string QueueUrl { get; } = queueUrl;

    public Type? ExplicitMessageType { get; private set; }

    public SqsConsumeConfiguration WithExplicitMessageType(Type explicitMessageType)
    {
        ExplicitMessageType = explicitMessageType;
        
        return this;
    }
}
