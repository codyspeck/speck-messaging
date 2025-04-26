using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Speck.DataflowExtensions;

namespace Speck.Messaging.Sqs;

internal class SqsConsumer(
    IAmazonSQS sqs,
    SqsConsumeConfiguration consumeConfiguration,
    SqsQueueUrls sqsQueueUrls,
    MessageReceiver messageReceiver)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = sqsQueueUrls.GetQueueUrl(consumeConfiguration.QueueName);
        
        await using var pipeline = DataflowPipelineBuilder.Create<List<Message>>()
            .Build(messages => ConsumeAsync(queueUrl, messages, stoppingToken));
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await sqs.ReceiveMessageAsync(
                new ReceiveMessageRequest
                {
                    MessageAttributeNames = { MessageHeaders.MessageType },
                    MaxNumberOfMessages = SqsMessagingConstants.BatchMessageLimit,
                    QueueUrl = queueUrl,
                },
                stoppingToken);

            if (response.Messages.Count == 0)
                continue;

            await pipeline.SendAsync(response.Messages);
        }
    }

    private async Task ConsumeAsync(string queueUrl, List<Message> messages, CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            messages
                .Select(message => new MessageEnvelope(message.Body)
                    .WithExplicitMessageType(consumeConfiguration.ExplicitMessageType)
                    .WithHeaders(message.MessageAttributes))
                .Select(envelope => messageReceiver.ReceiveAsync(envelope, cancellationToken)));
        
        await sqs.DeleteMessageBatchAsync(
            queueUrl,
            messages
                .Select(message => new DeleteMessageBatchRequestEntry(message.MessageId, message.ReceiptHandle))
                .ToList(),
            cancellationToken);
    }
}
