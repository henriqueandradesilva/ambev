using Ambev.DeveloperEvaluation.Domain.Events.Sales;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers.Interfaces;

public interface ISaleCancelledEventHandler
{
    Task Handle(
        SaleCancelledEvent notification,
        CancellationToken cancellationToken);
}