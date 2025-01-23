/// <summary>
/// Represents the result of an operation to update a sale.
/// </summary>
namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;

public class UpdateSaleResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the updated sale.
    /// </summary>
    public Guid Id { get; set; }
}