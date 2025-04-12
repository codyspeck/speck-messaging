using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

namespace Speck.Messaging.Kafka;

internal class KafkaConsumer(string queue, Wrapper<ConsumerConfig> config, MessageReceiver messageReceiver)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        
        using var consumer = new ConsumerBuilder<string, string>(config.Value).Build();
        
        consumer.Subscribe(queue);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);
            
            var messageEnvelope = new MessageEnvelope(consumeResult.Message.Value);

            foreach (var header in consumeResult.Message.Headers)
            {
                messageEnvelope.Headers.Add(header.Key, Encoding.UTF8.GetString(header.GetValueBytes()));
            }
            
            await messageReceiver.ReceiveAsync(messageEnvelope, stoppingToken);
        }
    }
}