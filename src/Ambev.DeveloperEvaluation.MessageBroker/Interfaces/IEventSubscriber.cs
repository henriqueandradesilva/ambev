namespace Ambev.DeveloperEvaluation.MessageBroker.Interfaces;

public interface IEventSubscriber<T>
{
    Task ConsumeAsync(
        T @event);
}
