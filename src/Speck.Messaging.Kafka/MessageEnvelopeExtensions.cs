using System.Text;
using Confluent.Kafka;

namespace Speck.Messaging.Kafka;

internal static class MessageEnvelopeExtensions
{
    public static MessageEnvelope WithHeaders(this MessageEnvelope messageEnvelope, Headers headers)
    {
        foreach (var header in headers)
        {
            messageEnvelope.Headers.Add(header.Key, Encoding.UTF8.GetString(header.GetValueBytes()));
        }

        return messageEnvelope;
    }    
}
