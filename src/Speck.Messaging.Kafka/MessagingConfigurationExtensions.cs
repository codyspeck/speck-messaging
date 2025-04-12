using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Speck.Messaging.Kafka;

public static class MessagingConfigurationExtensions
{
    public static MessagingConfiguration AddKafka(
        this MessagingConfiguration messagingConfiguration,
        Action<KafkaConfiguration> configure)
    {
        var kafkaConfiguration = new KafkaConfiguration();
        
        configure(kafkaConfiguration);
        
        RegisterServices(messagingConfiguration, kafkaConfiguration);
        
        return messagingConfiguration;
    }

    private static void RegisterServices(
        MessagingConfiguration messagingConfiguration,
        KafkaConfiguration kafkaConfiguration)
    {
        messagingConfiguration.Services.AddTransient(_ =>
        {
            var clientConfig = new ClientConfig();

            kafkaConfiguration.ConfigureAllClientsAction(clientConfig);
            
            return new Wrapper<ClientConfig>(clientConfig);
        });

        messagingConfiguration.Services.AddTransient(provider =>
        {
            var clientConfig = provider.GetRequiredService<Wrapper<ClientConfig>>();

            var consumerConfig = new ConsumerConfig(clientConfig.Value);

            kafkaConfiguration.ConfigureAllClientsAction(consumerConfig);

            return new Wrapper<ConsumerConfig>(consumerConfig);
        });

        foreach (var consumeConfiguration in kafkaConfiguration.ConsumeConfigurations)
        {
            messagingConfiguration.Services.AddSingleton<IHostedService>(provider =>
                new KafkaConsumer(
                    consumeConfiguration,
                    provider.GetRequiredService<Wrapper<ConsumerConfig>>(),
                    provider.GetRequiredService<MessageReceiver>()));
        }
        
        foreach (var sendToConfiguration in kafkaConfiguration.SendToConfigurations)
        {
            if (sendToConfiguration.CreateOnStartup)
            {
                messagingConfiguration.Services.AddSingleton<IHostedService>(provider =>
                    new KafkaQueueBootstrapService(
                        sendToConfiguration.Queue,
                        provider.GetRequiredService<Wrapper<ClientConfig>>()));
            }
            
            foreach (var messageType in sendToConfiguration.MessageTypes)
            {
                messagingConfiguration.EndpointFactories.Add(
                    messageType,
                    provider => new KafkaEndpoint(
                        sendToConfiguration.Queue,
                        provider.GetRequiredService<Wrapper<ClientConfig>>()));   
            }
        }
    }
}
