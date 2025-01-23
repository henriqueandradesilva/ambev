using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Serilog;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale, including its details, items, and associated operations.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Sale"/> class.
    /// </summary>
    public Sale()
    {
        Id = Guid.NewGuid();
        Number = GenerateSaleNumber();
        IsCancelled = false;
    }

    public Sale(
        Guid id)
    {
        Id = id;
        Number = GenerateSaleNumber();
        IsCancelled = false;
    }

    /// <summary>
    /// Gets or sets the name of the customer associated with the sale.
    /// </summary>
    public string Customer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch where the sale was made.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the sale.
    /// </summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the sale was made.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets the total amount for the sale, after applying discounts.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the sale has been cancelled.
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>
    /// Gets or sets the list of items associated with the sale.
    /// </summary>
    public List<SaleItem> ListSaleItems { get; set; } = new();

    #region Extensions

    /// <summary>
    /// Generates a 10-character unique alphanumeric sale number by combining 
    /// a GUID-derived segment with a randomly generated segment.
    /// </summary>
    /// <returns>A unique alphanumeric string consisting of 10 characters.</returns>
    /// <remarks>
    /// - The first 6 characters are derived from a GUID, ensuring uniqueness. 
    /// - The last 4 characters are randomly selected from a predefined alphanumeric set.
    /// - The final format is a blend of deterministic (GUID) and random elements, 
    ///   reducing the chance of collisions.
    /// </remarks>
    private string GenerateSaleNumber()
    {
        var random = new Random();

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        var guidPart = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

        var randomPart = new string(Enumerable.Repeat(chars, 4)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        return $"{guidPart}{randomPart}";
    }

    /// <summary>
    /// Validates the sale using the <see cref="SaleValidator"/>.
    /// </summary>
    /// <returns>A <see cref="ValidationResultDetail"/> containing the validation result and any errors found.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();

        var result = validator.Validate(this);

        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(e => (ValidationErrorDetail)e)
        };
    }

    /// <summary>
    /// Cancels the sale and updates its modification timestamp.
    /// </summary>
    /// <param name="isCancelled">Indicates whether the sale should be marked as cancelled.</param>
    public void CancelSale(
        bool isCancelled)
    {
        IsCancelled = isCancelled;

        if (isCancelled)
        {
            ListSaleItems.ForEach(x => x.Cancel());
            ListSaleItems.ForEach(x => x.MarkAsUpdated());
        }
    }

    /// <summary>
    /// Calculates the total amount for the sale, considering discounts and excluding cancelled items.
    /// </summary>
    public void CalculateTotalAmount()
    {
        ListSaleItems.ForEach(x => x.ApplyDiscount());

        TotalAmount =
            ListSaleItems.Where(item => !item.IsCancelled)
                         .Sum(item => item.TotalAmount);
    }

    /// <summary>
    /// Adds a new item to the sale, recalculates the total amount, and updates the modification timestamp.
    /// </summary>
    /// <param name="saleItem">The item to be added.</param>
    public void AddSaleItem(
        SaleItem saleItem)
    {
        ListSaleItems.Add(saleItem);

        CalculateTotalAmount();

        MarkAsUpdated();
    }

    /// <summary>
    /// Cancels a specific item in the sale, recalculates the total amount, and updates the sale's status if necessary.
    /// </summary>
    /// <param name="saleItemId">The unique identifier of the item to be cancelled.</param>
    /// <exception cref="KeyNotFoundException">Thrown if the item with the specified ID is not found.</exception>
    public void CancelSaleItem(
        Guid saleItemId)
    {
        var saleItem =
            ListSaleItems.FirstOrDefault(i => i.Id == saleItemId);

        if (saleItem == null)
        {
            var message = $"The sale item with ID {saleItemId} was not found.";

            Log.Error(message);

            throw new KeyNotFoundException(message);
        }

        saleItem.Cancel();

        CalculateTotalAmount();

        MarkAsUpdated();

        if (ListSaleItems.All(i => i.IsCancelled))
            IsCancelled = true;
    }

    #endregion
}