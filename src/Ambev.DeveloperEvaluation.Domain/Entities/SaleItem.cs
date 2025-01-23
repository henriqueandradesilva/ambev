using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Serilog;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item within a sale.
/// </summary>
public class SaleItem : BaseEntity
{
    public SaleItem()
    {
        Id = Guid.NewGuid();
        IsCancelled = false;
    }

    /// <summary>
    /// Gets or sets the identifier of the sale to which this item belongs.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the product name associated with this sale item.
    /// </summary>
    public string Product { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of the product being sold.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets the discount applied to this sale item.
    /// </summary>
    public decimal Discount { get; private set; }

    /// <summary>
    /// Gets the total amount for this sale item, after applying any discounts.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this sale item has been cancelled.
    /// </summary>
    public bool IsCancelled { get; private set; }

    #region Extensions

    /// <summary>
    /// Validates the sale item using the <see cref="SaleItemValidator"/>.
    /// </summary>
    /// <returns>A <see cref="ValidationResultDetail"/> containing the validation result and any errors found.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleItemValidator();

        var result = validator.Validate(this);

        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(e => (ValidationErrorDetail)e)
        };
    }

    /// <summary>
    /// Applies the appropriate discount based on the quantity of the product sold.
    /// Throws an exception if the quantity exceeds the allowed limit.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the quantity exceeds 20.</exception>
    public void ApplyDiscount()
    {
        if (Quantity > 20)
        {
            var message = "The quantity cannot exceed 20 items for a single product.";

            Log.Error(message);

            throw new InvalidOperationException(message);
        }

        decimal totalAmount = Quantity * UnitPrice;

        if (Quantity >= 4 && Quantity < 10)
            Discount = totalAmount * 0.1m;
        else if (Quantity >= 10 && Quantity <= 20)
            Discount = totalAmount * 0.2m;
        else
            Discount = 0m;

        TotalAmount = totalAmount - Discount;
    }

    /// <summary>
    /// Cancels this sale item.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
    }

    #endregion
}