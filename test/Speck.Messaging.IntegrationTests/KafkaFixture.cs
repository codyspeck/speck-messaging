using Testcontainers.Kafka;

namespace Speck.Messaging.IntegrationTests;

public static class KafkaFixture
{
    public static KafkaContainer KafkaContainer { get; } = new KafkaBuilder()
        .Build();

    [Before(TestSession)]
    public static async Task InitializeAsync()
    {
        await KafkaContainer.StartAsync();
    }

    [After(TestSession)]
    public static async Task DisposeAsync()
    {
        await KafkaContainer.StopAsync();
    }
}
