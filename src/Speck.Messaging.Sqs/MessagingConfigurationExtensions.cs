using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Speck.Messaging.Sqs;

public static class MessagingConfigurationExtensions
{
    public static MessagingConfiguration AddSqs(
        this MessagingConfiguration messagingConfiguration,
        Action<SqsConfiguration> configure)
    {
        var sqsConfiguration = new SqsConfiguration();
        
        configure(sqsConfiguration);
        
        ConfigureServices(messagingConfiguration, sqsConfiguration);
        
        return messagingConfiguration;
    }

    private static void ConfigureServices(
        MessagingConfiguration messagingConfiguration,
        SqsConfiguration sqsConfiguration)
    {
        foreach (var consumeConfiguration in sqsConfiguration.ConsumeConfigurations)
        {
            messagingConfiguration.Services.AddSingleton<IHostedService>(provider => new SqsConsumer(
                provider.GetRequiredService<IAmazonSQS>(),
                consumeConfiguration,
                provider.GetRequiredService<MessageReceiver>()));
        }
        
        foreach (var sendToConfiguration in sqsConfiguration.SendToConfigurations)
        {
            foreach (var messageType in sendToConfiguration.MessageTypes)
            {
                messagingConfiguration.EndpointFactories.Add(
                    messageType,
                    provider => new SqsEndpoint(
                        sendToConfiguration.QueueUrl,
                        provider.GetRequiredService<IAmazonSQS>()));
            }
        }
    }
}
