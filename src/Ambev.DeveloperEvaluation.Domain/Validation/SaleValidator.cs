using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for the <see cref="Sale"/> entity.
/// Ensures that all properties comply with business rules and system requirements.
/// </summary>
public class SaleValidator : AbstractValidator<Sale>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaleValidator"/> class.
    /// Defines the rules for validating the <see cref="Sale"/> entity.
    /// </summary>
    public SaleValidator()
    {
        // Rule: Customer is mandatory and must not exceed 150 characters.
        RuleFor(sale => sale.Customer)
            .NotEmpty().WithMessage("The customer name must be specified.")
            .MaximumLength(150).WithMessage("The customer name must not exceed 150 characters.");

        // Rule: Branch is mandatory and must not exceed 50 characters.
        RuleFor(sale => sale.Branch)
            .NotEmpty().WithMessage("The branch must be specified.")
            .MaximumLength(150).WithMessage("The branch name must not exceed 150 characters.");

        RuleFor(x => x.ListSaleItems)
            .NotEmpty().WithMessage("At least one item must be included in the sale.")
            .Must(items => items.Count() > 0).WithMessage("Items list must contain at least one item.")
            .ForEach(item => item.SetValidator(new SaleItemValidator()));
    }
}