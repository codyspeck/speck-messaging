using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;

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
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await sqs.ReceiveMessageAsync(
                new ReceiveMessageRequest
                {
                    MessageAttributeNames = { MessageHeaders.MessageType },
                    MaxNumberOfMessages = 10,
                    QueueUrl = queueUrl,
                },
                stoppingToken);

            foreach (var message in response.Messages)
            {
                var envelope = new MessageEnvelope(message.Body)
                    .WithExplicitMessageType(consumeConfiguration.ExplicitMessageType)
                    .WithHeaders(message.MessageAttributes);
                
                await messageReceiver.ReceiveAsync(envelope, stoppingToken);
            }
            
            await sqs.DeleteMessageBatchAsync(
                queueUrl,
                response.Messages
                    .Select(message => new DeleteMessageBatchRequestEntry(message.MessageId, message.ReceiptHandle))
                    .ToList(),
                stoppingToken);
        }
    }
}
