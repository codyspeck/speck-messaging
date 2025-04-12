using System.Text;
using System.Threading.Tasks.Dataflow;
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
            .Select(
                consumeResult => ReceiveMessageAsync(consumeResult, stoppingToken),
                new ExecutionDataflowBlockOptions
                {
                    CancellationToken = stoppingToken,
                    BoundedCapacity = consumeConfiguration.BoundedCapacity,
                    MaxDegreeOfParallelism = consumeConfiguration.MaxDegreeOfParallelism
                })
            .Build(consumer.StoreOffset);

        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);

            await consumePipeline.SendAsync(consumeResult);
        }
    }

    private async Task<ConsumeResult<string, string>> ReceiveMessageAsync(
        ConsumeResult<string, string> consumeResult,
        CancellationToken cancellationToken)
    {
        var messageEnvelope = new MessageEnvelope(consumeResult.Message.Value)
            .WithExplicitMessageType(consumeConfiguration.ExplicitMessageType)
            .WithHeaders(consumeResult.Message.Headers);

        await messageReceiver.ReceiveAsync(messageEnvelope, cancellationToken);

        return consumeResult;
    }
}
