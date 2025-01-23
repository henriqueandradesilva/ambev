using Ambev.DeveloperEvaluation.MessageBroker.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.MessageBroker.Brokers.RabbitMQ;

/// <summary>
/// Consumes messages from a RabbitMQ queue and processes them using a provided Subscriber implementation.
/// </summary>
/// <typeparam name="T">The type of the event/message being consumed.</typeparam>
public class RabbitMQEventSubscriber<T> where T : class
{
    private readonly RabbitMQConnector _connector;
    private readonly IEventSubscriber<T> _eventSubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQEventSubscriber{T}"/> class.
    /// </summary>
    /// <param name="connector">The RabbitMQ connector for managing connections and channels.</param>
    /// <param name="eventSubscriber">The Subscriber implementation for processing messages.</param>
    public RabbitMQEventSubscriber(
        RabbitMQConnector connector,
        IEventSubscriber<T> eventSubscriber)
    {
        _connector = connector ?? throw new ArgumentNullException(nameof(connector));
        _eventSubscriber = eventSubscriber ?? throw new ArgumentNullException(nameof(eventSubscriber));
    }

    /// <summary>
    /// Starts consuming messages from the specified RabbitMQ queue.
    /// </summary>
    /// <param name="queueName">The name of the queue to consume messages from.</param>
    /// <param name="cancellationToken">A token to cancel the consumption process.</param>
    public async Task ConsumeAsync(
        string queueName,
        CancellationToken cancellationToken = default)
    {
        var connection = await _connector.GetConnectionAsync();

        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var @event = JsonSerializer.Deserialize<T>(message);

                if (@event != null)
                    await _eventSubscriber.ConsumeAsync(@event);

                await channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");

                await channel.BasicNackAsync(ea.DeliveryTag, false, false);
            }
        };

        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer);

        await Task.Delay(-1, cancellationToken);
    }
}
