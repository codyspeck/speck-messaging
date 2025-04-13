using Confluent.Kafka;

namespace Speck.Messaging.Kafka;

public class KafkaConfiguration
{
    public Action<ClientConfig> ConfigureAllClientsAction { get; private set; } = _ => { };
    
    public Action<ConsumerConfig> ConfigureAllConsumerClientsAction { get; private set; } = _ => { };

    public List<KafkaConsumeConfiguration> ConsumeConfigurations { get; } = [];
    
    public List<KafkaSendToConfiguration> SendToConfigurations { get; } = [];

    public KafkaConfiguration ConfigureAllClients(Action<ClientConfig> configure)
    {
        ConfigureAllClientsAction = configure;
        
        return this;
    }
    
    public KafkaConfiguration ConfigureAllConsumerClients(Action<ConsumerConfig> configure)
    {
        ConfigureAllConsumerClientsAction = configure;
        
        return this;
    }

    public KafkaConfiguration ConsumeFrom(string queue)
    {
        return ConsumeFrom(queue, _ => { });
    }
    
    public KafkaConfiguration ConsumeFrom(string queue, Action<KafkaConsumeConfiguration> configure)
    {
        var consumeConfiguration = new KafkaConsumeConfiguration(queue);
        
        configure(consumeConfiguration);
        
        ConsumeConfigurations.Add(consumeConfiguration);

        return this;
    }
    
    public KafkaConfiguration SendTo(string queue, Action<KafkaSendToConfiguration> configure)
    {
        var sendToConfiguration = new KafkaSendToConfiguration(queue);

        configure(sendToConfiguration);
        
        SendToConfigurations.Add(sendToConfiguration);

        return this;
    }
}