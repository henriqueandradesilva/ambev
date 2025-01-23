using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;

/// <summary>
/// Validator for the <see cref="CreateSaleItemCommand"/> class.
/// Ensures that all required properties meet validation criteria.
/// </summary>
public class CreateSaleItemCommandValidator : AbstractValidator<CreateSaleItemCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleItemCommandValidator"/> class.
    /// </summary>
    public CreateSaleItemCommandValidator()
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