using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Validator for the <see cref="CreateSaleItemRequest"/> class.
/// Ensures that all properties of the sale item request meet validation criteria.
/// </summary>
public class CreateSaleItemRequestValidator : AbstractValidator<CreateSaleItemRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleItemRequestValidator"/> class.
    /// </summary>
    public CreateSaleItemRequestValidator()
    {
        RuleFor(x => x.Product)
            .NotEmpty()
            .WithMessage("The product name cannot be empty.")
            .Length(1, 150)
            .WithMessage("The product name must be between 1 and 150 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("The quantity must be at least 1.")
            .LessThanOrEqualTo(20)
            .WithMessage("You can only purchase up to 20 units of the same product.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("The unit price must be a positive value greater than zero.");
    }
}