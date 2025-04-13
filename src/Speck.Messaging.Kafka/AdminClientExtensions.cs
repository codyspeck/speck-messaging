using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace Speck.Messaging.Kafka;

internal static class AdminClientExtensions
{
    public static async Task TryCreateTopicAsync(this IAdminClient adminClient, string topic)
    {
        try
        {
            await adminClient.CreateTopicsAsync([new TopicSpecification { Name = topic }]);
        }
        catch (CreateTopicsException exception) when (exception.IsTopicAlreadyExistsException())
        {
            // Noop
        }
    }

    private static bool IsTopicAlreadyExistsException(this CreateTopicsException exception)
    {
        return exception.Results.All(result => result.Error.Code is ErrorCode.TopicAlreadyExists);
    }
}
