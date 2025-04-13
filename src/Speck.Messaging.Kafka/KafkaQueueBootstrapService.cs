using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

namespace Speck.Messaging.Kafka;

internal class KafkaQueueBootstrapService(string queue, Wrapper<AdminClientConfig> adminClientConfig) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var adminClient = new AdminClientBuilder(adminClientConfig.Value).Build();

        await adminClient.TryCreateTopicAsync(queue);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}