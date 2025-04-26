using Amazon.SQS;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Speck.Messaging.Sqs;

internal class SqsQueueCreationService(
    string queueName,
    IAmazonSQS sqs,
    ILogger<SqsQueueCreationService>? logger) :
    IHostedLifecycleService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        logger?.LogInformation("Ensuring SQS queue {QueueName} exists.", queueName);
        
        await sqs.CreateQueueAsync(queueName, cancellationToken);
    }

    public Task StartedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StoppingAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
