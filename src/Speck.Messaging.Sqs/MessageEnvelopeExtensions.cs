using Amazon.SQS.Model;

namespace Speck.Messaging.Sqs;

internal static class MessageEnvelopeExtensions
{
    private const string StringDataType = "String";
    
    public static Dictionary<string, MessageAttributeValue> ToMessageAttributes(this MessageEnvelope messageEnvelope)
    {
        return messageEnvelope.Headers.ToDictionary(
            header => header.Key,
            header => new MessageAttributeValue
            {
                DataType = StringDataType,
                StringValue = header.Value
            });
    }

    public static MessageEnvelope WithHeaders(
        this MessageEnvelope messageEnvelope,
        IDictionary<string, MessageAttributeValue> headers)
    {
        foreach (var header in headers)
        {
            messageEnvelope.Headers.Add(header.Key, header.Value.StringValue);
        }

        return messageEnvelope;
    }
}
