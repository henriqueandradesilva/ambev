using Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

using Serilog;

/// <summary>
/// Handler responsible for processing the cancellation of a sale.
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleCancelledEventHandler _saleCancelledEventHandler;

    /// <summary>
    /// Initializes the handler with the required repositories.
    /// </summary>
    /// <param name="saleRepository">The sale repository used for managing sale data.</param>
    /// <param name="saleCancelledEventHandler">The event handler responsible for processing sale cancellation events.</param>
    public CancelSaleHandler(
        ISaleRepository saleRepository,
        ISaleCancelledEventHandler saleCancelledEventHandler)
    {
        _saleRepository = saleRepository;
        _saleCancelledEventHandler = saleCancelledEventHandler;
    }

    /// <summary>
    /// Handles the cancellation of a sale.
    /// </summary>
    /// <param name="request">The cancellation command containing the sale ID to be canceled.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    /// <returns>A result indicating the outcome of the cancellation operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the sale does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the sale is already cancelled.</exception>
    public async Task<CancelSaleResult> Handle(
        CancelSaleCommand request,
        CancellationToken cancellationToken)
    {
        // Retrieve the sale from the repository including its items.
        var existingSale =
            await _saleRepository.GetByIdWithIncludeAsync(request.Id, cancellationToken, x => x.ListSaleItems);

        // Throw an exception if the sale does not exist.
        if (existingSale == null)
        {
            var message = $"Sale with ID {request.Id} not found.";
            Log.Error(message);
            throw new KeyNotFoundException(message);
        }

        // Throw an exception if the sale has already been cancelled.
        if (existingSale.IsCancelled)
        {
            var message = "Sale is already cancelled.";
            Log.Error(message);
            throw new InvalidOperationException(message);
        }

        // Cancel the sale and mark it as updated.
        existingSale.CancelSale(true);
        existingSale.MarkAsUpdated();

        // Update the sale in the repository.
        await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        // Trigger the sale cancellation event.
        await _saleCancelledEventHandler.Handle(new SaleCancelledEvent(existingSale), cancellationToken);

        // Return a successful result indicating that the sale was cancelled.
        return new CancelSaleResult
        {
            Success = true,
            Message = "The sale was successfully cancelled."
        };
    }
}