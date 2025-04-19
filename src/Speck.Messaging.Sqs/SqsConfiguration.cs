namespace Speck.Messaging.Sqs;

public class SqsConfiguration
{
    internal List<SqsConsumeConfiguration> ConsumeConfigurations { get; } = [];

    internal List<SqsSendToConfiguration> SendToConfigurations { get; } = [];

    public SqsConfiguration ConsumeFrom(string queueUrl, Action<SqsConsumeConfiguration> configure)
    {
        var sqsConsumeConfiguration = new SqsConsumeConfiguration(queueUrl);
        
        configure(sqsConsumeConfiguration);
        
        ConsumeConfigurations.Add(sqsConsumeConfiguration);

        return this;
    }
    
    public SqsConfiguration SendTo(string queueUrl, Action<SqsSendToConfiguration> configure)
    {
        var sqsSendToConfiguration = new SqsSendToConfiguration(queueUrl);
        
        configure(sqsSendToConfiguration);

        SendToConfigurations.Add(sqsSendToConfiguration);
        
        return this;
    }
}
