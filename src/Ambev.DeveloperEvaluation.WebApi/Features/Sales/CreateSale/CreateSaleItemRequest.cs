namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents a request to create a sale item in a sale operation.
/// </summary>
public class CreateSaleItemRequest
{
    /// <summary>
    /// Gets or sets the name of the product being sold.
    /// </summary>
    public string Product { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of the product being sold.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product being sold.
    /// </summary>
    public decimal UnitPrice { get; set; }
}