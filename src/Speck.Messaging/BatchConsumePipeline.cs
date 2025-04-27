using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Speck.DataflowExtensions;

namespace Speck.Messaging;

internal sealed class BatchConsumePipeline<TMessage> : IConsumePipeline, IAsyncDisposable
{
    private readonly IServiceScopeFactory _factory;
    private readonly DataflowPipeline<Completable<TMessage>> _pipeline;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public BatchConsumePipeline(
        BatchConsumerConfiguration batchConsumerConfiguration,
        IServiceScopeFactory factory,
        Wrapper<CancellationTokenSource> cancellationTokenSource)
    {
        _factory = factory;
        _cancellationTokenSource = cancellationTokenSource.Value;
        _pipeline = DataflowPipelineBuilder.Create<Completable<TMessage>>()
            .Batch(
                batchConsumerConfiguration.BatchSize,
                TimeSpan.FromMilliseconds(batchConsumerConfiguration.BatchTimeoutInMilliseconds))
            .Build(ConsumeAsync, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = batchConsumerConfiguration.BoundedCapacity,
                MaxDegreeOfParallelism = batchConsumerConfiguration.MaxDegreeOfParallelism
            });
    }
    
    public async Task SendAsync(object message)
    {
        await _pipeline.SendAndWaitForCompletionAsync((TMessage)message);
    }

    private async Task ConsumeAsync(Completable<TMessage>[] messages)
    {
        try
        {
            await using var scope = _factory.CreateAsyncScope();

            var consumer = scope.ServiceProvider.GetRequiredService<IBatchConsumer<TMessage>>();

            await consumer.ConsumeAsync(
                messages.Select(message => message.Value).ToArray(),
                _cancellationTokenSource.Token);

            foreach (var message in messages)
            {
                message.Complete();
            }
        }
        catch (Exception exception)
        {
            foreach (var message in messages)
            {
                message.Fail(exception);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _pipeline.DisposeAsync();
    }
}
