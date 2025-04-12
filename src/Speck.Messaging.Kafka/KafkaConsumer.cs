using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Speck.DataflowExtensions;

namespace Speck.Messaging.Kafka;

internal class KafkaConsumer(
    KafkaConsumeConfiguration consumeConfiguration,
    Wrapper<ConsumerConfig> config,
    MessageReceiver messageReceiver)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(config.Value).Build();
        
        consumer.Subscribe(consumeConfiguration.Queue);

        await using var consumePipeline = DataflowPipelineBuilder.Create<ConsumeResult<string, string>>()
            .Select(async consumeResult =>
            {
                var messageEnvelope = new MessageEnvelope(
                    consumeResult.Message.Value,
                    consumeConfiguration.ExplicitMessageType);

                foreach (var header in consumeResult.Message.Headers)
                {
                    messageEnvelope.Headers.Add(header.Key, Encoding.UTF8.GetString(header.GetValueBytes()));
                }

                await messageReceiver.ReceiveAsync(messageEnvelope, stoppingToken);

                return consumeResult;
            })
            .Build(consumer.StoreOffset);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);
            
            await consumePipeline.SendAsync(consumeResult);
        }
    }
}
