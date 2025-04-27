using Microsoft.Extensions.DependencyInjection;

namespace Speck.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, Action<MessagingConfiguration> configure)
    {
        var messagingConfiguration = new MessagingConfiguration(services);
        
        configure(messagingConfiguration);

        RegisterServices(services, messagingConfiguration);
        
        return services;
    }

    private static void RegisterServices(IServiceCollection services, MessagingConfiguration messagingConfiguration)
    {
        services.AddSingleton<Func<Type, IEndpoint>>(
            provider => messageType => messagingConfiguration.EndpointFactories[messageType](provider));
        
        services.AddSingleton<Func<Type, IConsumePipeline>>(
            provider => messageType => messagingConfiguration.ConsumerFactories[messageType](provider));
        
        services.AddSingleton(messagingConfiguration.EndpointFactories);

        services.AddSingleton(messagingConfiguration.MessageTypeRegistry);
        
        services.AddSingleton<IMessageSender, MessageSender>();
        
        services.AddSingleton<MessageReceiver>();

        services.AddSingleton<IMessageSerializer>(new DefaultMessageSerializer());
        
        services.AddSingleton(new Wrapper<CancellationTokenSource>(new CancellationTokenSource()));

        services.AddHostedService<HostStoppingService>();
    }
}
