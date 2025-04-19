namespace Speck.Messaging.Sqs;

public class SqsSendToConfiguration(string queueName)
{
    public string QueueName { get; } = queueName;
    
    public List<Type> MessageTypes { get; } = [];

    public SqsSendToConfiguration Message<TMessage>()
    {
        MessageTypes.Add(typeof(TMessage));

        return this;
    }
}
