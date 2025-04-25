namespace Speck.Messaging.Sqs;

internal class SqsQueueUrls
{
    private readonly Dictionary<string, string> _queueUrls = [];

    public string GetQueueUrl(string queueName)
    {
        return _queueUrls[queueName];
    }

    public void SetQueueUrl(string queueName, string queueUrl)
    {
        _queueUrls[queueName] = queueUrl;
    }
}
