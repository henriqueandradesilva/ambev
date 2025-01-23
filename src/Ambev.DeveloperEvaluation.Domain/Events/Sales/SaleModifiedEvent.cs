using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Interfaces;

namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

/// <summary>
/// Event triggered when a sale is modified.
/// </summary>
public class SaleModifiedEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaleModifiedEvent"/> class.
    /// </summary>
    /// <param name="sale">The sale that was modified.</param>
    public SaleModifiedEvent(
        Sale sale)
    {
        Sale = sale;
    }

    /// <summary>
    /// Gets the sale that was modified.
    /// </summary>
    public Sale Sale { get; }
}
