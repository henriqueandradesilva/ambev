using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSale;

/// <summary>
/// Represents a command to cancel a specific item within a sale, identified by its unique ID and the associated sale ID.
/// </summary>
public class CancelSaleItemCommand : IRequest<CancelSaleItemResult>
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale containing the item to be canceled.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the item to be canceled.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelSaleItemCommand"/> class with the specified item and sale IDs.
    /// </summary>
    /// <param name="id">The unique identifier of the item to cancel.</param>
    /// <param name="saleId">The unique identifier of the sale containing the item.</param>
    public CancelSaleItemCommand(
        Guid saleId,
        Guid id)
    {
        SaleId = saleId;
        Id = id;
    }
}