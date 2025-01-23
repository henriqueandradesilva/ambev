using Ambev.DeveloperEvaluation.Domain.Events.Sales;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers.Interfaces;

public interface ISaleCreatedEventHandler
{
    Task Handle(
        SaleCreatedEvent notification,
        CancellationToken cancellationToken);
}