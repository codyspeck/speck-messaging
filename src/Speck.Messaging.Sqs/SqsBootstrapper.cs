using Amazon.SQS;
using Microsoft.Extensions.Hosting;

namespace Speck.Messaging.Sqs;

internal class SqsBootstrapper(IEnumerable<string> sqsQueueNames, IAmazonSQS sqs, SqsQueueUrls sqsQueueUrls)
    : IHostedLifecycleService
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
        foreach (var sqsQueueName in sqsQueueNames)
        {
            var response = await sqs.GetQueueUrlAsync(sqsQueueName, cancellationToken);
            
            sqsQueueUrls.SetQueueUrl(sqsQueueName, response.QueueUrl);
        }
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
