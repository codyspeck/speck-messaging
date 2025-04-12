namespace Speck.Messaging.Kafka;

public class KafkaSendToConfiguration(string queue)
{
    public string Queue { get; } = queue;

    public bool CreateOnStartup { get; private set; }

    public List<Type> MessageTypes { get; } = [];

    public KafkaSendToConfiguration CreateOnStartupIf(bool condition)
    {
        CreateOnStartup = condition;
        
        return this;
    }
    
    public KafkaSendToConfiguration Message<TMessage>()
    {
        MessageTypes.Add(typeof(TMessage));
        
        return this;
    }
}
