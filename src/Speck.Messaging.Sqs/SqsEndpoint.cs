using Amazon.SQS;

namespace Speck.Messaging.Sqs;

internal class SqsEndpoint(IAmazonSQS sqs) : IEndpoint
{
    public Task SendAsync(MessageEnvelope messageEnvelope)
    {
        throw new NotImplementedException();
    }
}
