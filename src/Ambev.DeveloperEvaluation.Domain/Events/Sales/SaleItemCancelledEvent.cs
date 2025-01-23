using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Interfaces;

namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

/// <summary>
/// Event triggered when a sale item is cancelled.
/// </summary>
public class SaleItemCancelledEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItemCancelledEvent"/> class.
    /// </summary>
    /// <param name="sale">The sale associated with the cancelled item.</param>
    /// <param name="saleItem">The sale item that was cancelled.</param>
    public SaleItemCancelledEvent(
        Sale sale,
        SaleItem saleItem)
    {
        Sale = sale;
        SaleItem = saleItem;
    }

    /// <summary>
    /// Gets the sale associated with the cancelled item.
    /// </summary>
    public Sale Sale { get; }

    /// <summary>
    /// Gets the sale item that was cancelled.
    /// </summary>
    public SaleItem SaleItem { get; }
}