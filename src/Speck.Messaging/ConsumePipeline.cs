using Microsoft.Extensions.DependencyInjection;

namespace Speck.Messaging;

internal class ConsumePipeline<TMessage>(IServiceScopeFactory factory) : IConsumePipeline
{
    public async Task SendAsync(object message, CancellationToken cancellationToken)
    {
        await using var scope = factory.CreateAsyncScope();
        
        var consumer = scope.ServiceProvider.GetRequiredService<IConsumer<TMessage>>();
        
        await consumer.ConsumeAsync((TMessage)message, cancellationToken);
    }
}
