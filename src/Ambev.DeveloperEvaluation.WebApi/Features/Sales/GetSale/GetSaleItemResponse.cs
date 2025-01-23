namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// API response model for GetSaleItemResponse operation
/// </summary>
public class GetSaleItemResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the sale associated with the item.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string Product { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of the product sold.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount applied to the product.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets or sets the total amount for the sale item, including discount.
    /// </summary>
    public decimal TotalAmount { get; set; }
}