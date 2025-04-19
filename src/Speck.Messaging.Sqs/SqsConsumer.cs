using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;

namespace Speck.Messaging.Sqs;

internal class SqsConsumer(
    IAmazonSQS sqs,
    SqsConsumeConfiguration consumeConfiguration,
    MessageReceiver messageReceiver)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await sqs.ReceiveMessageAsync(
                new ReceiveMessageRequest
                {
                    MessageAttributeNames = { MessageHeaders.MessageType },
                    MaxNumberOfMessages = 10,
                    QueueUrl = consumeConfiguration.QueueUrl,
                },
                stoppingToken);

            foreach (var message in response.Messages)
            {
                var envelope = new MessageEnvelope(message.Body)
                    .WithExplicitMessageType(consumeConfiguration.ExplicitMessageType)
                    .WithHeaders(message.MessageAttributes);
                
                await messageReceiver.ReceiveAsync(envelope, stoppingToken);
            }
        }
    }
}
