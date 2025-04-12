using Confluent.Kafka;

namespace Speck.Messaging.Kafka;

internal sealed class KafkaEndpoint(string queue, Wrapper<ClientConfig> clientConfig) : IEndpoint, IDisposable
{
    private readonly IProducer<string, string> _producer = new ProducerBuilder<string, string>(clientConfig.Value)
        .Build();
    
    public async Task SendAsync(MessageEnvelope messageEnvelope)
    {
        await _producer.ProduceAsync(queue, new Message<string, string>
        {
            Headers = messageEnvelope.GetHeaders(),
            Value = messageEnvelope.Body,
        });
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}
