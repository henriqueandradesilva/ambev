using Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.MessageBroker.Interfaces;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers;

/// <summary>
/// Handles the event when a sale is modified by publishing it to the event queue.
/// </summary>
public class SaleModifiedEventHandler : MessageBroker.EventHandler<SaleModifiedEvent>, ISaleModifiedEventHandler
{
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleModifiedEventHandler"/> class.
    /// </summary>
    /// <param name="eventPublisher">The event Publisher for publishing events to a message queue.</param>
    public SaleModifiedEventHandler(
        IEventPublisher eventPublisher) : base(eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Handles the <see cref="SaleModifiedEvent"/> by publishing it to the appropriate message queue.
    /// </summary>
    /// <param name="notification">The sale modification event data.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task Handle(
        SaleModifiedEvent notification,
        CancellationToken cancellationToken)
    {
        await _eventPublisher.PublishToQueueAsync(notification, nameof(SaleModifiedEvent));
    }
}