namespace Speck.Messaging;

internal class Wrapper<T>(T value)
{
    public T Value { get; } = value;
}
