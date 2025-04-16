using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Speck.DataflowExtensions;

namespace Speck.Messaging;

internal sealed class BatchConsumePipeline<TMessage> : IConsumePipeline, IAsyncDisposable
{
    private readonly IServiceScopeFactory _factory;
    private readonly DataflowPipeline<DataflowItem<(TMessage, CancellationToken)>> _pipeline;

    public BatchConsumePipeline(BatchConsumerConfiguration batchConsumerConfiguration, IServiceScopeFactory factory)
    {
        _factory = factory;
        _pipeline = DataflowPipelineBuilder.Create<DataflowItem<(TMessage, CancellationToken)>>()
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
        var dataflowItem = new DataflowItem<(TMessage, CancellationToken)>(((TMessage)message, cancellationToken));
        await _pipeline.SendAsync(dataflowItem);
        await dataflowItem.Task;
    }

    private async Task ConsumeAsync(DataflowItem<(TMessage Message, CancellationToken CancellationToken)>[] items)
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
                item.Error(exception);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _pipeline.DisposeAsync();
    }
}
