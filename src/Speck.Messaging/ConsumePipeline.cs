using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Speck.DataflowExtensions;

namespace Speck.Messaging;

internal sealed class ConsumePipeline<TMessage> : IConsumePipeline, IAsyncDisposable
{
    private readonly IServiceScopeFactory _factory;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly DataflowPipeline<Completable<TMessage>> _pipeline;

    public ConsumePipeline(
        ConsumerConfiguration consumerConfiguration,
        IServiceScopeFactory factory,
        Wrapper<CancellationTokenSource> cancellationTokenSource)
    {
        _factory = factory;
        _cancellationTokenSource = cancellationTokenSource.Value;
        _pipeline = DataflowPipelineBuilder.Create<Completable<TMessage>>()
            .Build(ConsumeAsync, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = consumerConfiguration.BoundedCapacity,
                MaxDegreeOfParallelism = consumerConfiguration.MaxDegreeOfParallelism
            });
    }
    
    public async Task SendAsync(object message)
    {
        await _pipeline.SendAndWaitForCompletionAsync((TMessage)message);
    }

    private async Task ConsumeAsync(Completable<TMessage> message)
    {
        try
        {
            await using var scope = _factory.CreateAsyncScope();

            var consumer = scope.ServiceProvider.GetRequiredService<IConsumer<TMessage>>();

            await consumer.ConsumeAsync(message.Value, _cancellationTokenSource.Token);

            message.Complete();
        }
        catch (Exception exception)
        {
            message.Fail(exception);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _pipeline.DisposeAsync();
    }
}
