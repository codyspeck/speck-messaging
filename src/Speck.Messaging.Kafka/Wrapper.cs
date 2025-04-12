namespace Speck.Messaging.Kafka;

internal class Wrapper<T>(T value)
{
    internal T Value { get; } = value;
}
