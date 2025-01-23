using Ambev.DeveloperEvaluation.Domain.Events.Sales;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers.Interfaces;

public interface ISaleModifiedEventHandler
{
    Task Handle(
        SaleModifiedEvent notification,
        CancellationToken cancellationToken);
}