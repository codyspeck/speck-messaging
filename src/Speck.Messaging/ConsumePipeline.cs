using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Speck.DataflowExtensions;

namespace Speck.Messaging;

internal sealed class ConsumePipeline<TMessage> : IConsumePipeline, IAsyncDisposable
{
    private readonly IServiceScopeFactory _factory;
    private readonly DataflowPipeline<Completable<(TMessage, CancellationToken)>> _pipeline;

    public ConsumePipeline(ConsumerConfiguration consumerConfiguration, IServiceScopeFactory factory)
    {
        _factory = factory;
        _pipeline = DataflowPipelineBuilder.Create<Completable<(TMessage, CancellationToken)>>()
            .Build(ConsumeAsync, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = consumerConfiguration.BoundedCapacity,
                MaxDegreeOfParallelism = consumerConfiguration.MaxDegreeOfParallelism
            });
    }
    
    public async Task SendAsync(object message, CancellationToken cancellationToken)
    {
        await _pipeline.SendAndWaitForCompletionAsync(((TMessage)message, cancellationToken));
    }

    private async Task ConsumeAsync(Completable<(TMessage Message, CancellationToken CancellationToken)> item)
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
            item.Fail(exception);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _pipeline.DisposeAsync();
    }
}
