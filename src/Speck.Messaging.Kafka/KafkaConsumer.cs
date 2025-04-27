using System.Threading.Tasks.Dataflow;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Speck.DataflowExtensions;

namespace Speck.Messaging.Kafka;

internal class KafkaConsumer(
    KafkaConsumeConfiguration consumeConfiguration,
    Wrapper<AdminClientConfig> adminClientConfig,
    Wrapper<ConsumerConfig> consumerConfig,
    MessageReceiver messageReceiver)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (consumeConfiguration.CreateOnStartup)
        {
            using var admin = new AdminClientBuilder(adminClientConfig.Value).Build();

            await admin.TryCreateTopicAsync(consumeConfiguration.Queue);
        }
        
        using var consumer = new ConsumerBuilder<string, string>(consumerConfig.Value).Build();
        
        consumer.Subscribe(consumeConfiguration.Queue);

        await using var consumePipeline = DataflowPipelineBuilder.Create<ConsumeResult<string, string>>()
            .Select(ReceiveMessageAsync, new ExecutionDataflowBlockOptions
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

    private async Task<ConsumeResult<string, string>> ReceiveMessageAsync(ConsumeResult<string, string> consumeResult)
    {
        var messageEnvelope = new MessageEnvelope(consumeResult.Message.Value)
            .WithExplicitMessageType(consumeConfiguration.ExplicitMessageType)
            .WithHeaders(consumeResult.Message.Headers);

        await messageReceiver.ReceiveAsync(messageEnvelope);

        return consumeResult;
    }
}
