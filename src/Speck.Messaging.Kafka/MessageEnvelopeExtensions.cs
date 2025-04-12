using System.Text;
using Confluent.Kafka;

namespace Speck.Messaging.Kafka;

internal static class MessageEnvelopeExtensions
{
    public static Headers GetHeaders(this MessageEnvelope envelope)
    {
        var headers = new Headers();

        foreach (var header in envelope.Headers)
        {
            headers.Add(header.Key, Encoding.UTF8.GetBytes(header.Value));
        }

        return headers;
    }
    
    public static MessageEnvelope WithHeaders(this MessageEnvelope messageEnvelope, Headers headers)
    {
        foreach (var header in headers)
        {
            messageEnvelope.Headers.Add(header.Key, Encoding.UTF8.GetString(header.GetValueBytes()));
        }

        return messageEnvelope;
    }    
}
