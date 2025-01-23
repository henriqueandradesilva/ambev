using Ambev.DeveloperEvaluation.Domain.Events.Sales;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers.Interfaces;

public interface ISaleItemCancelledEventHandler
{
    Task Handle(
        SaleItemCancelledEvent notification,
        CancellationToken cancellationToken);
}