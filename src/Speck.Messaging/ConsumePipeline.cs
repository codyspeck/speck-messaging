using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Speck.DataflowExtensions;

namespace Speck.Messaging;

internal class ConsumePipeline<TMessage> : IConsumePipeline
{
    private readonly IServiceScopeFactory _factory;
    private readonly DataflowPipeline<(TMessage, CancellationToken)> _pipeline;

    public ConsumePipeline(ConsumerConfiguration consumerConfiguration, IServiceScopeFactory factory)
    {
        _factory = factory;
        _pipeline = DataflowPipelineBuilder.Create<(TMessage, CancellationToken)>()
            .Build(ConsumeAsync, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = consumerConfiguration.BoundedCapacity,
                MaxDegreeOfParallelism = consumerConfiguration.MaxDegreeOfParallelism
            });
    }
    
    public async Task SendAsync(object message, CancellationToken cancellationToken)
    {
        await _pipeline.SendAsync(((TMessage)message, cancellationToken));
    }

    private async Task ConsumeAsync((TMessage Message, CancellationToken CancellationToken) item)
    {
        await using var scope = _factory.CreateAsyncScope();
        
        var consumer = scope.ServiceProvider.GetRequiredService<IConsumer<TMessage>>();
        
        await consumer.ConsumeAsync(item.Message, item.CancellationToken);
    }
}
