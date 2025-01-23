using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;

/// <summary>
/// Validator for the <see cref="CreateSaleCommand"/> class.
/// </summary>
/// <remarks>
/// Ensures that the sale command data meets the necessary business rules and constraints.
/// This includes validating fields such as date, customer, branch, and the list of sale items.
/// </remarks>
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleCommandValidator"/> class.
    /// </summary>
    /// <remarks>
    /// Defines the validation rules for the <see cref="CreateSaleCommand"/>.
    /// </remarks>
    public CreateSaleCommandValidator()
    {
        RuleFor(sale => sale.Date)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("The sale date is required and cannot be empty.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("The sale date cannot be a future date.");

        RuleFor(sale => sale.Customer)
            .NotEmpty().WithMessage("The customer name must be provided.")
            .MinimumLength(3).WithMessage("The customer name must have at least 3 characters.")
            .MaximumLength(150).WithMessage("The customer name must have no more than 150 characters.");

        RuleFor(sale => sale.Branch)
            .NotEmpty().WithMessage("The branch name must be provided.")
            .MinimumLength(3).WithMessage("The branch name must have at least 3 characters.")
            .MaximumLength(150).WithMessage("The branch name must have no more than 150 characters.");

        RuleFor(x => x.ListSaleItems)
            .NotEmpty()
            .WithMessage("The sale must include at least one item.");

        RuleForEach(x => x.ListSaleItems)
            .SetValidator(new CreateSaleItemCommandValidator());
    }
}