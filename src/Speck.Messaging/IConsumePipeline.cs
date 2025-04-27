namespace Speck.Messaging;

internal interface IConsumePipeline
{
    Task SendAsync(object message);
}
