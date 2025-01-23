using Ambev.DeveloperEvaluation.MessageBroker.Interfaces;

namespace Ambev.DeveloperEvaluation.MessageBroker;

public class EventHandler<T> : IEventHandler<T> where T : class
{
    private readonly IEventPublisher _eventPublisher;

    public EventHandler(
        IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public virtual async Task Handle(
        T notification,
        CancellationToken cancellationToken)
    {
        try
        {
            var queueName = typeof(T).Name;

            await _eventPublisher.PublishToQueueAsync(notification, queueName);
        }
        catch (Exception ex)
        {
            var errorMessage = new
            {
                Success = false,
                Message = $"{ex.Message}-{ex.InnerException?.Message}"
            };

            await _eventPublisher.PublishToQueueAsync(errorMessage, "errorQueue");

            throw;
        }
    }
}