namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetListSale;

/// <summary>
/// Represents the result of a GetListSale operation.
/// </summary>
public class GetListSaleResult
{
    /// <summary>
    /// The unique identifier of the sale.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The customer name.
    /// </summary>
    public string Customer { get; set; } = string.Empty;

    /// <summary>
    /// The branch name.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// The sale number.
    /// </summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// The sale date.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// The total amount of the sale.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// The list of sale items.
    /// </summary>
    public List<GetListSaleItemResult> ListSaleItems { get; set; } = new();
}