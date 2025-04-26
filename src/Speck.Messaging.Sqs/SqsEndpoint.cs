using Amazon.SQS;
using Amazon.SQS.Model;

namespace Speck.Messaging.Sqs;

internal class SqsEndpoint(string queueUrl, IAmazonSQS sqs) : IEndpoint
{
    public async Task SendAsync(MessageEnvelope messageEnvelope)
    {
        await sqs.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageAttributes = messageEnvelope.ToMessageAttributes(),
            MessageBody = messageEnvelope.Body
        });
    }
}
