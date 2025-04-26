namespace Speck.Messaging.Sqs;

public class SqsConfiguration
{
    internal List<string> QueuesToCreate { get; } = [];

    internal List<SqsConsumeConfiguration> ConsumeConfigurations { get; } = [];

    internal List<SqsSendToConfiguration> SendToConfigurations { get; } = [];

    public SqsConfiguration ConsumeFrom(string queueName, Action<SqsConsumeConfiguration> configure)
    {
        var sqsConsumeConfiguration = new SqsConsumeConfiguration(queueName);
        
        configure(sqsConsumeConfiguration);
        
        ConsumeConfigurations.Add(sqsConsumeConfiguration);

        return this;
    }

    public SqsConfiguration EnsureQueueCreated(string queueName)
    {
        QueuesToCreate.Add(queueName);
        
        return this;
    }
    
    public SqsConfiguration SendTo(string queueName, Action<SqsSendToConfiguration> configure)
    {
        var sqsSendToConfiguration = new SqsSendToConfiguration(queueName);
        
        configure(sqsSendToConfiguration);

        SendToConfigurations.Add(sqsSendToConfiguration);
        
        return this;
    }
}
