using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Serilog;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.DeleteSale;

/// <summary>
/// Handles the deletion of a sale in the system.
/// </summary>
public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResult>
{
    private readonly ISaleRepository _saleRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSaleHandler"/> class.
    /// </summary>
    /// <param name="saleRepository">The repository for handling sale operations.</param>
    public DeleteSaleHandler(
        ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    /// <summary>
    /// Handles the delete sale command.
    /// </summary>
    /// <param name="request">The delete sale command containing the sale ID to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    public async Task<DeleteSaleResult> Handle(
        DeleteSaleCommand request,
        CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleCommandValidator();

        var validationResult =
            await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(Environment.NewLine, validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

            Log.Error("Validation failed: {ValidationErrors}", message);

            throw new ValidationException(validationResult.Errors);
        }

        var success =
            await _saleRepository.DeleteAsync(request.Id, cancellationToken);

        if (!success)
        {
            var message = $"Sale with ID {request.Id} not found";

            Log.Error(message);

            throw new KeyNotFoundException(message);
        }

        return new DeleteSaleResult()
        {
            Success = success,
            Message = "The sale was successfully deleted."
        };
    }
}