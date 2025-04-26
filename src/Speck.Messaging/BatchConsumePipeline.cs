using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Speck.DataflowExtensions;

namespace Speck.Messaging;

internal sealed class BatchConsumePipeline<TMessage> : IConsumePipeline, IAsyncDisposable
{
    private readonly IServiceScopeFactory _factory;
    private readonly DataflowPipeline<Completable<(TMessage, CancellationToken)>> _pipeline;

    public BatchConsumePipeline(BatchConsumerConfiguration batchConsumerConfiguration, IServiceScopeFactory factory)
    {
        _factory = factory;
        _pipeline = DataflowPipelineBuilder.Create<Completable<(TMessage, CancellationToken)>>()
            .Batch(
                batchConsumerConfiguration.BatchSize,
                TimeSpan.FromMilliseconds(batchConsumerConfiguration.BatchTimeoutInMilliseconds))
            .Build(ConsumeAsync, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = batchConsumerConfiguration.BoundedCapacity,
                MaxDegreeOfParallelism = batchConsumerConfiguration.MaxDegreeOfParallelism
            });
    }
    
    public async Task SendAsync(object message, CancellationToken cancellationToken)
    {
        await _pipeline.SendAndWaitForCompletionAsync(((TMessage)message, cancellationToken));
    }

    private async Task ConsumeAsync(Completable<(TMessage Message, CancellationToken CancellationToken)>[] items)
    {
        try
        {
            await using var scope = _factory.CreateAsyncScope();

            var consumer = scope.ServiceProvider.GetRequiredService<IBatchConsumer<TMessage>>();

            await consumer.ConsumeAsync(items.Select(item => item.Value.Message).ToArray());

            foreach (var item in items)
            {
                item.Complete();
            }
        }
        catch (Exception exception)
        {
            foreach (var item in items)
            {
                item.Fail(exception);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _pipeline.DisposeAsync();
    }
}
