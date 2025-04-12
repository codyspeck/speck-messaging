using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Hosting;

namespace Speck.Messaging.Kafka;

internal class KafkaQueueBootstrapService(string queue, Wrapper<ClientConfig> clientConfig) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var adminClient = new AdminClientBuilder(clientConfig.Value).Build();

        await adminClient.CreateTopicsAsync([new TopicSpecification { Name = queue }]);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}