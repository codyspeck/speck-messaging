namespace Speck.Messaging;

internal class DataflowItem<TValue>(TValue value)
{
    private readonly TaskCompletionSource _taskCompletionSource = new();
    
    public TValue Value { get; } = value;

    public Task Task => _taskCompletionSource.Task;
    
    public void Complete() => _taskCompletionSource.TrySetResult();
    
    public void Error(Exception exception) => _taskCompletionSource.TrySetException(exception);
}

