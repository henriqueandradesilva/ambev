using Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.MessageBroker.Interfaces;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers;

/// <summary>
/// Handles the event when a sale is created by publishing it to the event queue.
/// </summary>
public class SaleCreatedEventHandler : MessageBroker.EventHandler<SaleCreatedEvent>, ISaleCreatedEventHandler
{
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleCreatedEventHandler"/> class.
    /// </summary>
    /// <param name="eventPublisher">The event Publisher for publishing events to a message queue.</param>
    public SaleCreatedEventHandler(
        IEventPublisher eventPublisher) : base(eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Handles the <see cref="SaleCreatedEvent"/> by publishing it to the appropriate message queue.
    /// </summary>
    /// <param name="notification">The sale creation event data.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task Handle(
        SaleCreatedEvent notification,
        CancellationToken cancellationToken)
    {
        await _eventPublisher.PublishToQueueAsync(notification, nameof(SaleCreatedEvent));
    }
}