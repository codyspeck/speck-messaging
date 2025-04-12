namespace Speck.Messaging;

public interface IMessageSender
{
    Task SendAsync(object message);
}
