using System.Text;
using Confluent.Kafka;

namespace Speck.Messaging.Kafka;

internal sealed class KafkaEndpoint(string queue, Wrapper<ClientConfig> clientConfig) : IEndpoint, IDisposable
{
    private readonly IProducer<string, string> _producer = new ProducerBuilder<string, string>(clientConfig.Value)
        .Build();
    
    public async Task SendAsync(MessageEnvelope messageEnvelope)
    {
        var headers = new Headers();
        
        foreach (var header in messageEnvelope.Headers)
        {
            headers.Add(header.Key, Encoding.UTF8.GetBytes(header.Value));
        }
        
        var message = new Message<string, string>
        {
            Headers = headers,
            Value = messageEnvelope.Body,
        };

        await _producer.ProduceAsync(queue, message);
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}
