namespace Speck.Messaging.Sqs;

public class SqsSendToConfiguration(string queueUrl)
{
    public string QueueUrl { get; } = queueUrl;
    
    public List<Type> MessageTypes { get; } = [];

    public SqsSendToConfiguration Message<TMessage>()
    {
        MessageTypes.Add(typeof(TMessage));

        return this;
    }
}
