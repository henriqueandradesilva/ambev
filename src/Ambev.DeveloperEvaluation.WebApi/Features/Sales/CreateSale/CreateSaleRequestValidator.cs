using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Validator for <see cref="CreateSaleRequest"/> that ensures all required fields are properly populated and meet the validation rules.
/// </summary>
public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleRequestValidator"/> class.
    /// Configures the validation rules for creating a sale request.
    /// </summary>
    public CreateSaleRequestValidator()
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
            .WithMessage("The sale must include at least one item.")
            .ForEach(item => item.SetValidator(new CreateSaleItemRequestValidator()));
    }
}