namespace Speck.Messaging;

public interface IEndpoint
{
    Task SendAsync(MessageEnvelope messageEnvelope);
}
