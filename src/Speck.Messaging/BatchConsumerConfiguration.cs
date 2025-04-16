namespace Speck.Messaging;

public class BatchConsumerConfiguration
{
    public int BatchSize { get; private set; } = 1;

    public int BatchTimeoutInMilliseconds { get; private set; } = 5;
    
    public int BoundedCapacity { get; private set; } = 1;
    
    public int MaxDegreeOfParallelism { get; private set; } = 1;

    public BatchConsumerConfiguration WithBatchSize(int batchSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(batchSize);
        
        BatchSize = batchSize;
        
        return this;
    }

    public BatchConsumerConfiguration WithBatchTimeoutInMilliseconds(int batchTimeoutInMilliseconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(batchTimeoutInMilliseconds);
        
        BatchTimeoutInMilliseconds = batchTimeoutInMilliseconds;
        
        return this;
    }
    
    public BatchConsumerConfiguration WithBoundedCapacity(int boundedCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfZero(boundedCapacity);
        
        BoundedCapacity = boundedCapacity;
        
        return this;
    }

    public BatchConsumerConfiguration WithMaxDegreeOfParallelism(int maxDegreeOfParallelism)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxDegreeOfParallelism);
        
        MaxDegreeOfParallelism = maxDegreeOfParallelism;
        
        return this;
    }
}