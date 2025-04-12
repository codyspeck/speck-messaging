namespace Speck.Messaging;

public class ConsumerConfiguration
{
    public int BoundedCapacity { get; private set; } = 1;
    
    public int MaxDegreeOfParallelism { get; private set; } = 1;

    public ConsumerConfiguration WithBoundedCapacity(int boundedCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfZero(boundedCapacity);
        
        BoundedCapacity = boundedCapacity;
        
        return this;
    }

    public ConsumerConfiguration WithMaxDegreeOfParallelism(int maxDegreeOfParallelism)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxDegreeOfParallelism);
        
        MaxDegreeOfParallelism = maxDegreeOfParallelism;
        
        return this;
    }
}