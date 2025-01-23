namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;

/// <summary>
/// Represents the result of a sale item creation operation.
/// </summary>
public class CreateSaleItemResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the associated sale.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the product associated with the sale item.
    /// </summary>
    public string Product { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of the product in the sale item.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product in the sale item.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount applied to the sale item.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets or sets the total amount for the sale item after discount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the sale item has been cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }
}