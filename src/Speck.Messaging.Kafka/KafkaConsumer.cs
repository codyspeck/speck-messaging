using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

namespace Speck.Messaging.Kafka;

internal class KafkaConsumer(
    KafkaConsumeConfiguration consumeConfiguration,
    Wrapper<ConsumerConfig> config,
    MessageReceiver messageReceiver)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        
        using var consumer = new ConsumerBuilder<string, string>(config.Value).Build();
        
        consumer.Subscribe(consumeConfiguration.Queue);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);
            
            var messageEnvelope = new MessageEnvelope(
                consumeResult.Message.Value,
                consumeConfiguration.ExplicitMessageType);

            foreach (var header in consumeResult.Message.Headers)
            {
                messageEnvelope.Headers.Add(header.Key, Encoding.UTF8.GetString(header.GetValueBytes()));
            }
            
            await messageReceiver.ReceiveAsync(messageEnvelope, stoppingToken);
            
            consumer.StoreOffset(consumeResult);
        }
    }
}