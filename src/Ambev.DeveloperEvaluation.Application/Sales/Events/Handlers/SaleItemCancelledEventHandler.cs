using Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.MessageBroker.Interfaces;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers;

/// <summary>
/// Handles the event when a sale item is cancelled by publishing it to the event queue.
/// </summary>
public class SaleItemCancelledEventHandler : MessageBroker.EventHandler<SaleItemCancelledEvent>, ISaleItemCancelledEventHandler
{
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItemCancelledEventHandler"/> class.
    /// </summary>
    /// <param name="eventPublisher">The event Publisher for publishing events to a message queue.</param>
    public SaleItemCancelledEventHandler(
        IEventPublisher eventPublisher) : base(eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Handles the <see cref="SaleItemCancelledEvent"/> by publishing it to the appropriate message queue.
    /// </summary>
    /// <param name="notification">The sale item cancellation event data.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task Handle(
        SaleItemCancelledEvent notification,
        CancellationToken cancellationToken)
    {
        await _eventPublisher.PublishToQueueAsync(notification, nameof(SaleItemCancelledEvent));
    }
}