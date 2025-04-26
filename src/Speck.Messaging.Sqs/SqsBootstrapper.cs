using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Speck.Messaging.Sqs;

internal class SqsBootstrapper(
    IEnumerable<string> sqsQueueNamesToCreate,
    IEnumerable<string> sqsQueueNames,
    IAmazonSQS sqs,
    SqsQueueUrls sqsQueueUrls,
    ILogger<SqsBootstrapper>? logger) :
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
        foreach (var sqsQueueName in sqsQueueNamesToCreate)
        {
            logger?.LogInformation("Ensuring SQS queue exists: {Queue}", sqsQueueName);

            try
            {
                await sqs.CreateQueueAsync(sqsQueueName, cancellationToken);
            }
            catch (QueueNameExistsException)
            {
                // Noop
            }
        }
        
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
