namespace Speck.Messaging.Sqs;

public class SqsConfiguration
{
    internal List<SqsSendToConfiguration> SendToConfigurations { get; } = [];
    
    public SqsConfiguration SendTo(string queueUrl, Action<SqsSendToConfiguration> configure)
    {
        var sqsSendToConfiguration = new SqsSendToConfiguration(queueUrl);
        
        configure(sqsSendToConfiguration);

        SendToConfigurations.Add(sqsSendToConfiguration);
        
        return this;
    }
}
