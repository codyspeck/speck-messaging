namespace Speck.Messaging.Sqs;

public static class MessagingConfigurationExtensions
{
    public static MessagingConfiguration AddSqs(
        this MessagingConfiguration messagingConfiguration,
        Action<SqsConfiguration> configure)
    {
        var sqsConfiguration = new SqsConfiguration();
        
        configure(sqsConfiguration);
        
        return messagingConfiguration;
    }

    private static void ConfigureServices(
        MessagingConfiguration messagingConfiguration,
        SqsConfiguration sqsConfiguration)
    {
        foreach (var sendToConfiguration in sqsConfiguration.SendToConfigurations)
        {
            messagingConfiguration.EndpointFactories
        }
    }
}
