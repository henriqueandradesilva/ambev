using Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Serilog;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSale;

/// <summary>
/// Handles the cancellation of a specific item within a sale.
/// </summary>
public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleItemCancelledEventHandler _saleItemCancelledEventHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelSaleItemHandler"/> class.
    /// </summary>
    /// <param name="saleRepository">The repository to manage sales data.</param>
    /// <param name="saleItemCancelledEventHandler">The handler responsible for processing the event when a sale item is cancelled.</param>
    public CancelSaleItemHandler(
        ISaleRepository saleRepository,
        ISaleItemCancelledEventHandler saleItemCancelledEventHandler)
    {
        _saleRepository = saleRepository;
        _saleItemCancelledEventHandler = saleItemCancelledEventHandler;
    }

    /// <summary>
    /// Processes the cancellation of a sale item.
    /// </summary>
    /// <param name="request">The command containing the sale and sale item IDs to be cancelled.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="CancelSaleItemResult"/> indicating the outcome of the operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the sale or sale item is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the sale or sale item is already cancelled.</exception>
    public async Task<CancelSaleItemResult> Handle(
        CancelSaleItemCommand request,
        CancellationToken cancellationToken)
    {
        var existingSale =
            await _saleRepository.GetByIdWithIncludeAsync(request.SaleId, cancellationToken, x => x.ListSaleItems);

        if (existingSale == null)
        {
            var message = $"Sale with ID {request.SaleId} not found.";

            Log.Error(message);

            throw new KeyNotFoundException(message);
        }

        if (existingSale.IsCancelled)
        {
            var message = "Sale is already cancelled.";

            Log.Error(message);

            throw new InvalidOperationException(message);
        }

        var existingSaleItem =
            existingSale.ListSaleItems.FirstOrDefault(i => i.Id == request.Id);

        if (existingSaleItem == null)
        {
            var message = $"Sale Item with ID {request.Id} not found in sale.";

            Log.Error(message);

            throw new KeyNotFoundException(message);
        }

        if (existingSaleItem.IsCancelled)
        {
            var message = "Sale item is already cancelled.";

            Log.Error(message);

            throw new InvalidOperationException(message);
        }

        existingSale.CancelSaleItem(request.Id);

        existingSale.MarkAsUpdated();
        existingSaleItem.MarkAsUpdated();

        await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        await _saleItemCancelledEventHandler.Handle(
            new SaleItemCancelledEvent(existingSale, existingSaleItem), cancellationToken);

        return new CancelSaleItemResult
        {
            Success = true,
            Message = "The sale item was successfully cancelled."
        };
    }
}