namespace Speck.Messaging;

public interface IMessageSerializer
{
    object Deserialize(string message, Type type);
    
    string Serialize(object message);
}
