namespace Ambev.DeveloperEvaluation.MessageBroker.Interfaces;

public interface IEventHandler<T>
{
    Task Handle(
        T notification,
        CancellationToken cancellationToken);
}