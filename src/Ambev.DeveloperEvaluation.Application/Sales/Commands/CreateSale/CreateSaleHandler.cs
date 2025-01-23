using Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Serilog;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;

/// <summary>
/// Handler responsible for processing the CreateSaleCommand to create a new sale.
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleCreatedEventHandler _saleCreatedEventHandler;

    /// <summary>
    /// Initializes the handler with the required repositories.
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="eventHandler">The sale created event handler</param>
    public CreateSaleHandler(
        ISaleRepository saleRepository,
        ISaleCreatedEventHandler saleCreatedEventHandler)
    {
        _saleRepository = saleRepository;
        _saleCreatedEventHandler = saleCreatedEventHandler;
    }

    /// <summary>
    /// Handles the CreateSaleCommand request to create a new sale.
    /// </summary>
    /// <param name="command">The CreateSale command containing necessary data for sale creation</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The result of the sale creation, including the sale ID</returns>
    /// <exception cref="ValidationException">Thrown if the command validation fails</exception>
    public async Task<CreateSaleResult> Handle(
        CreateSaleCommand command,
        CancellationToken cancellationToken)
    {
        // Validate the command
        var validator =
            new CreateSaleCommandValidator();

        var validationResult =
            await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(Environment.NewLine, validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

            Log.Error("Validation failed: {ValidationErrors}", errors);

            throw new ValidationException(validationResult.Errors);
        }

        // Create the sale and its items
        var sale = new Sale
        {
            Customer = command.Customer,
            Branch = command.Branch,
            Date = command.Date
        };

        foreach (var item in command.ListSaleItems)
        {
            sale.ListSaleItems.Add(new SaleItem
            {
                Product = item.Product,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            });
        }

        // Apply discounts and calculate the total amount for the sale
        sale.ListSaleItems.ForEach(item => item.ApplyDiscount());
        sale.CalculateTotalAmount();

        // Save the sale in the repository
        await _saleRepository.CreateAsync(sale, cancellationToken);

        // Publish the sale created event
        var saleCreatedEvent = new SaleCreatedEvent(sale);
        await _saleCreatedEventHandler.Handle(saleCreatedEvent, cancellationToken);

        // Return the result with the created sale ID
        return new CreateSaleResult { Id = sale.Id };
    }
}