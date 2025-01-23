using Ambev.DeveloperEvaluation.MessageBroker.Interfaces;
using RabbitMQ.Client;
using Serilog;
using System.Text;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.MessageBroker.Brokers.RabbitMQ;

/// <summary>
/// Publishes messages to RabbitMQ queues.
/// </summary>
public class RabbitMQEventPublisher : IEventPublisher
{
    private readonly RabbitMQConnector _connector;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQEventPublisher"/> class.
    /// </summary>
    /// <param name="connector">The RabbitMQ connector for managing connections and channels.</param>
    public RabbitMQEventPublisher(
        RabbitMQConnector connector)
    {
        _connector = connector ?? throw new ArgumentNullException(nameof(connector));
    }

    /// <summary>
    /// Publishes a message to a specified RabbitMQ queue.
    /// </summary>
    /// <typeparam name="T">The type of the message to publish.</typeparam>
    /// <param name="message">The message to be published.</param>
    /// <param name="queueName">The name of the queue to publish to.</param>
    public async Task PublishToQueueAsync<T>(
        T message,
        string queueName) where T : class
    {
        if (string.IsNullOrWhiteSpace(queueName))
        {
            var error = $"Queue name cannot be null or empty, {queueName}";

            Log.Error(error);

            throw new ArgumentException(error);
        }

        try
        {
            var connection =
                await _connector.GetConnectionAsync();

            using var channel =
                await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: true,
                basicProperties: properties,
                body: body);

            Console.WriteLine($"Message published to queue '{queueName}': {typeof(T).Name}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to publish message to queue '{queueName}': {ex.Message}");
        }
    }

    /// <summary>
    /// Publishes a message to a specified RabbitMQ topic.
    /// Not yet implemented.
    /// </summary>
    /// <typeparam name="T">The type of the message to publish.</typeparam>
    /// <param name="message">The message to be published.</param>
    /// <param name="topicName">The name of the topic to publish to.</param>
    public Task PublishToTopicAsync<T>(
        T message,
        string topicName) where T : class
    {
        var error = "Publishing to topics is not yet implemented.";

        Log.Error(error);

        throw new NotImplementedException(error);
    }
}