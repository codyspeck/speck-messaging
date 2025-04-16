using Microsoft.Extensions.DependencyInjection;

namespace Speck.Messaging;

public class MessagingConfiguration(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services;
    
    public Dictionary<Type, Func<IServiceProvider, IEndpoint>> EndpointFactories { get; } = [];

    internal Dictionary<Type, Func<IServiceProvider, IConsumePipeline>> ConsumerFactories { get; } = [];
    
    internal MessageTypeRegistry MessageTypeRegistry { get; } = new();

    public MessagingConfiguration AddBatchConsumer<TMessage, TConsumer>(Action<BatchConsumerConfiguration> configure)
        where TConsumer : class, IBatchConsumer<TMessage>
    {
        var batchConsumerConfiguration = new BatchConsumerConfiguration();
        
        configure(batchConsumerConfiguration);

        Services.AddTransient<IBatchConsumer<TMessage>, TConsumer>();
        
        Services.AddSingleton<BatchConsumePipeline<TMessage>>(provider => new BatchConsumePipeline<TMessage>(
            batchConsumerConfiguration,
            provider.GetRequiredService<IServiceScopeFactory>()));
        
        ConsumerFactories.Add(
            typeof(TMessage),
            provider => provider.GetRequiredService<BatchConsumePipeline<TMessage>>());

        return this;
    }
    
    public MessagingConfiguration AddConsumer<TMessage, TConsumer>(Action<ConsumerConfiguration> configure)
        where TConsumer : class, IConsumer<TMessage>
    {
        var consumerConfiguration = new ConsumerConfiguration();
        
        configure(consumerConfiguration);
        
        Services.AddTransient<IConsumer<TMessage>, TConsumer>();
        
        Services.AddSingleton<ConsumePipeline<TMessage>>(provider => new ConsumePipeline<TMessage>(
            consumerConfiguration,
            provider.GetRequiredService<IServiceScopeFactory>()));
        
        ConsumerFactories.Add(
            typeof(TMessage),
            provider => provider.GetRequiredService<ConsumePipeline<TMessage>>());

        return this;
    }
    
    public MessagingConfiguration AddMessage<TMessage>(string messageTypeName)
    {
        MessageTypeRegistry.Add<TMessage>(messageTypeName);

        return this;
    }
}
