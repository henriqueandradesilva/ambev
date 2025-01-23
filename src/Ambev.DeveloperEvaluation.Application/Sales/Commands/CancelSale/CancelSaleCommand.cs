using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSale;

/// <summary>
/// Represents a command to cancel a sale, identified by its unique ID.
/// </summary>
public class CancelSaleCommand : IRequest<CancelSaleResult>
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale to be canceled.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelSaleCommand"/> class with the specified sale ID.
    /// </summary>
    /// <param name="id">The unique identifier of the sale to cancel.</param>
    public CancelSaleCommand(
        Guid id)
    {
        Id = id;
    }
}