using Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Serilog;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;

/// <summary>
/// Handler responsible for processing the UpdateSaleCommand to update an existing sale.
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleModifiedEventHandler _saleModifiedEventHandler;

    /// <summary>
    /// Initializes the handler with the required repositories.
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="saleModifiedEventHandler">The event handler for sale modification</param>
    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        ISaleModifiedEventHandler saleModifiedEventHandler)
    {
        _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
        _saleModifiedEventHandler = saleModifiedEventHandler ?? throw new ArgumentNullException(nameof(saleModifiedEventHandler));
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request to update an existing sale.
    /// </summary>
    /// <param name="command">The UpdateSale command containing the updated sale data</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The result of the sale update, including the sale ID</returns>
    /// <exception cref="ValidationException">Thrown if the command validation fails</exception>
    /// <exception cref="KeyNotFoundException">Thrown if the sale is not found</exception>
    /// <exception cref="InvalidOperationException">Thrown if the sale is already cancelled</exception>
    public async Task<UpdateSaleResult> Handle(
        UpdateSaleCommand command,
        CancellationToken cancellationToken)
    {
        // Validate the request
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(Environment.NewLine, validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            Log.Error("Validation failed: {ValidationErrors}", errors);
            throw new ValidationException(validationResult.Errors);
        }

        // Retrieve the existing sale with its items
        var existingSale = await _saleRepository.GetByIdWithIncludeAsync(command.Id, cancellationToken, x => x.ListSaleItems);

        if (existingSale == null)
        {
            var message = $"Sale with ID {command.Id} not found";
            Log.Error(message);
            throw new KeyNotFoundException(message);
        }

        // If the sale is already cancelled, it cannot be updated
        if (existingSale.IsCancelled)
        {
            var message = "Sale is already cancelled.";
            Log.Error(message);
            throw new InvalidOperationException(message);
        }

        // Update sale properties
        existingSale.Customer = command.Customer;
        existingSale.Branch = command.Branch;
        existingSale.Date = command.Date;
        existingSale.CancelSale(command.IsCancelled);
        existingSale.MarkAsUpdated();

        // Save the updated sale in the repository
        await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        // Publish the sale modified event
        await _saleModifiedEventHandler.Handle(new SaleModifiedEvent(existingSale), cancellationToken);

        // Return the result with the updated sale ID
        return new UpdateSaleResult { Id = existingSale.Id };
    }
}