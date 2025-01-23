using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Interfaces;

namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

/// <summary>
/// Event triggered when a sale is created.
/// </summary>
public class SaleCreatedEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaleCreatedEvent"/> class.
    /// </summary>
    /// <param name="sale">The sale that was created.</param>
    public SaleCreatedEvent(
        Sale sale)
    {
        Sale = sale;
    }

    /// <summary>
    /// Gets the sale that was created.
    /// </summary>
    public Sale Sale { get; }
}
