namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents a request to create a sale, including customer, branch, date, and sale items.
/// </summary>
public class CreateSaleRequest
{
    /// <summary>
    /// Gets or sets the customer name associated with the sale.
    /// </summary>
    public string Customer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch where the sale occurred.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of the sale.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the list of sale items included in the sale.
    /// </summary>
    public List<CreateSaleItemRequest> ListSaleItems { get; set; } = new();
}