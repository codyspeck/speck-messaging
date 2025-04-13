using System.Diagnostics.CodeAnalysis;
using Confluent.Kafka;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Testing;
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
    [Experimental("EXTEXP0016")]
    public async Task Test()
    {
        var handledMessages = new HandledMessages();
        
        using var host = FakeHost.CreateBuilder()
            .ConfigureServices(services => services
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
                        .ConfigureAllConsumerClients(config =>
                        {
                            config.GroupId = "test";
                            config.AutoOffsetReset = AutoOffsetReset.Earliest;
                        })
                        .ConsumeFrom("yeet", consumer => consumer
                            .CreateOnStartupIf(true))
                        .SendTo("yeet", sender => sender
                            .CreateOnStartupIf(true)
                            .Message<TestMessage>()))))
            .Build();

        await host.StartAsync();
        
        var testMessage = TestMessage.Create();
        
        await host.Services
            .GetRequiredService<IMessageSender>()
            .SendAsync(testMessage);

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