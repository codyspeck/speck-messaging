using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        messagingConfiguration.Services.AddSingleton<SqsQueueUrls>();
        
        messagingConfiguration.Services.AddHostedService(provider => new SqsBootstrapper(
            sqsConfiguration.QueuesToCreate,
            sqsConfiguration.ConsumeConfigurations
                .Select(configuration => configuration.QueueName)
                .Concat(sqsConfiguration.SendToConfigurations
                    .Select(configuration => configuration.QueueName)),
            provider.GetRequiredService<IAmazonSQS>(),
            provider.GetRequiredService<SqsQueueUrls>(),
            provider.GetService<ILogger<SqsBootstrapper>>()));
        
        foreach (var consumeConfiguration in sqsConfiguration.ConsumeConfigurations)
        {
            messagingConfiguration.Services.AddSingleton<IHostedService>(provider => new SqsConsumer(
                provider.GetRequiredService<IAmazonSQS>(),
                consumeConfiguration,
                provider.GetRequiredService<SqsQueueUrls>(),
                provider.GetRequiredService<MessageReceiver>()));
        }
        
        foreach (var sendToConfiguration in sqsConfiguration.SendToConfigurations)
        {
            foreach (var messageType in sendToConfiguration.MessageTypes)
            {
                messagingConfiguration.EndpointFactories.Add(
                    messageType,
                    provider => new SqsEndpoint(
                        provider
                            .GetRequiredService<SqsQueueUrls>()
                            .GetQueueUrl(sendToConfiguration.QueueName),
                        provider.GetRequiredService<IAmazonSQS>()));
            }
        }
    }
}
