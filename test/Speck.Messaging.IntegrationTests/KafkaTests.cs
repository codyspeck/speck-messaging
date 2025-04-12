using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;
using Speck.Messaging.Kafka;

namespace Speck.Messaging.IntegrationTests;

public class KafkaTests
{
    private static readonly ResiliencePipeline Retry = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions())
        .Build();
    
    [Test]
    public async Task Test()
    {
        var handledMessages = new HandledMessages();
        
        await using var provider = new ServiceCollection()
            .AddSingleton(handledMessages)
            .AddMessaging(messaging => messaging
                .AddMessage<TestMessage>("test-message")
                .AddConsumer<TestMessage, TestMessageConsumer>(consumer => consumer
                    .WithBoundedCapacity(1)
                    .WithMaxDegreeOfParallelism(1))
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

        var testMessage = TestMessage.Create();
        
        await sender.SendAsync(testMessage);

        Retry.Execute(() => handledMessages.Should().Contain(testMessage));
    }

    private class HandledMessages : List<TestMessage>;

    private record TestMessage(Guid Id)
    {
        public static TestMessage Create() => new(Guid.NewGuid());
    };

    private class TestMessageConsumer(HandledMessages handledMessages) : IConsumer<TestMessage>
    {
        public Task ConsumeAsync(TestMessage message, CancellationToken cancellationToken)
        {
            handledMessages.Add(message);
            return Task.CompletedTask;
        }
    }
}