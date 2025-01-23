using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.DeleteSale;

/// <summary>
/// Command to delete a sale by its unique identifier.
/// </summary>
public class DeleteSaleCommand : IRequest<DeleteSaleResult>
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale to be deleted.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSaleCommand"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the sale to be deleted.</param>
    public DeleteSaleCommand(
        Guid id)
    {
        Id = id;
    }
}