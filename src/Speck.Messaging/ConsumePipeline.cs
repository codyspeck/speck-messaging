using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Speck.DataflowExtensions;

namespace Speck.Messaging;

internal sealed class ConsumePipeline<TMessage> : IConsumePipeline, IAsyncDisposable
{
    private readonly IServiceScopeFactory _factory;
    private readonly DataflowPipeline<DataflowItem<(TMessage, CancellationToken)>> _pipeline;

    public ConsumePipeline(ConsumerConfiguration consumerConfiguration, IServiceScopeFactory factory)
    {
        _factory = factory;
        _pipeline = DataflowPipelineBuilder.Create<DataflowItem<(TMessage, CancellationToken)>>()
            .Build(ConsumeAsync, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = consumerConfiguration.BoundedCapacity,
                MaxDegreeOfParallelism = consumerConfiguration.MaxDegreeOfParallelism
            });
    }
    
    public async Task SendAsync(object message, CancellationToken cancellationToken)
    {
        var dataflowItem = new DataflowItem<(TMessage, CancellationToken)>(((TMessage)message, cancellationToken));
        await _pipeline.SendAsync(dataflowItem);
        await dataflowItem.Task;
    }

    private async Task ConsumeAsync(DataflowItem<(TMessage Message, CancellationToken CancellationToken)> item)
    {
        try
        {
            await using var scope = _factory.CreateAsyncScope();

            var consumer = scope.ServiceProvider.GetRequiredService<IConsumer<TMessage>>();

            await consumer.ConsumeAsync(item.Value.Message, item.Value.CancellationToken);

            item.Complete();
        }
        catch (Exception exception)
        {
            item.Error(exception);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _pipeline.DisposeAsync();
    }
}
