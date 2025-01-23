using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for the <see cref="SaleItem"/> entity.
/// Ensures that the properties of a sale item adhere to the business rules and requirements.
/// </summary>
public class SaleItemValidator : AbstractValidator<SaleItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItemValidator"/> class.
    /// Defines the validation rules for the <see cref="SaleItem"/> entity.
    /// </summary>
    public SaleItemValidator()
    {
        // Rule: Product name must not be empty and must not exceed 150 characters.
        RuleFor(item => item.Product)
            .NotEmpty().WithMessage("The product name must be specified.")
            .MaximumLength(150).WithMessage("The product name must not exceed 150 characters.");

        // Rule: Unit price must be greater than 0.
        RuleFor(item => item.UnitPrice)
            .GreaterThan(0).WithMessage("The unit price must be greater than 0.");

        // Rule: Quantity must be greater than 0 and must not exceed 20.
        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("The quantity must be greater than 0.")
            .LessThanOrEqualTo(20).WithMessage("The quantity must not exceed 20.");

        // Rule: Discount must not be negative.
        RuleFor(item => item.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("The discount cannot be negative.");

        // Custom Rule: Validates that the discount adheres to the business rules.
        RuleFor(item => item)
            .Must(BeValidDiscount).WithMessage("The discount does not follow the business rules.");

        // Rule: Total amount must not be negative.
        RuleFor(item => item.TotalAmount)
            .GreaterThanOrEqualTo(0).WithMessage("The total amount cannot be negative.");
    }

    /// <summary>
    /// Validates the discount based on the quantity and unit price.
    /// Ensures that:
    /// - No discount for quantities less than 4.
    /// - 10% discount for quantities between 4 and 9.
    /// - 20% discount for quantities between 10 and 20.
    /// </summary>
    /// <param name="item">The <see cref="SaleItem"/> to validate.</param>
    /// <returns>True if the discount is valid; otherwise, false.</returns>
    private bool BeValidDiscount(
        SaleItem item)
    {
        // No discount allowed for quantities less than 4.
        if (item.Quantity < 4 && item.Discount > 0)
            return false;

        // 10% discount for quantities between 4 and 9.
        if (item.Quantity >= 4 && item.Quantity < 10 &&
            item.Discount != item.Quantity * item.UnitPrice * 0.1m)
            return false;

        // 20% discount for quantities between 10 and 20.
        if (item.Quantity >= 10 && item.Quantity <= 20 &&
            item.Discount != item.Quantity * item.UnitPrice * 0.2m)
            return false;

        return true;
    }
}