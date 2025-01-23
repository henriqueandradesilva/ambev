namespace Ambev.DeveloperEvaluation.MessageBroker.Interfaces;

public interface IEventPublisher
{
    Task PublishToQueueAsync<T>(
        T message,
        string queueName) where T : class;

    Task PublishToTopicAsync<T>(
        T message,
        string topicName) where T : class;
}