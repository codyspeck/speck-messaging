using Microsoft.Extensions.DependencyInjection;

namespace Speck.Messaging;

public class MessagingConfiguration(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services;
    
    public Dictionary<Type, Func<IServiceProvider, IEndpoint>> EndpointFactories { get; } = [];

    internal Dictionary<Type, Func<IServiceProvider, IConsumerWrapper>> ConsumerFactories { get; } = [];
    
    internal MessageTypeRegistry MessageTypeRegistry { get; } = new();

    public MessagingConfiguration AddConsumer<TMessage, TConsumer>()
        where TConsumer : class, IConsumer<TMessage>
    {
        Services.AddTransient<TConsumer>();

        ConsumerFactories.Add(
            typeof(TMessage),
            provider => new ConsumerWrapper<TMessage>(provider.GetRequiredService<TConsumer>()));

        return this;
    }
    
    public MessagingConfiguration AddMessage<TMessage>(string messageTypeName)
    {
        MessageTypeRegistry.Add<TMessage>(messageTypeName);

        return this;
    }
}
