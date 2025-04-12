using Microsoft.Extensions.DependencyInjection;
using Speck.Messaging.Kafka;

namespace Speck.Messaging.IntegrationTests;

public class Tests
{
    [Test]
    public async Task Test()
    {
        await using var provider = new ServiceCollection()
            .AddMessaging(messaging => messaging
                .AddMessage<TestMessage>("test-message")
                .AddConsumer<TestMessage, TestMessageConsumer>()
                .AddKafka(kafka => kafka
                    .ConfigureAllClients(config =>
                    {
                        config.BootstrapServers = KafkaFixture.KafkaContainer.GetBootstrapAddress();
                    })
                    .ConsumeFrom("yeet")
                    .SendTo("yeet", sender => sender
                        .CreateOnStartupIf(true)
                        .Message<TestMessage>())))
            .BuildServiceProvider();
        
        var sender = provider.GetRequiredService<IMessageSender>();
        
        await sender.SendAsync(new TestMessage());

        await Task.Delay(5000);
    }

    private class TestMessage;

    private class TestMessageConsumer : IConsumer<TestMessage>
    {
        public Task ConsumeAsync(TestMessage message, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Consumed message: {message}");
            return Task.CompletedTask;
        }
    }
}