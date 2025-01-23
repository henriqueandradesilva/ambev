using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.DeleteSale;

/// <summary>
/// Validator for the <see cref="DeleteSaleCommand"/> to ensure its properties meet the required criteria.
/// </summary>
public class DeleteSaleCommandValidator : AbstractValidator<DeleteSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSaleCommandValidator"/> class.
    /// </summary>
    public DeleteSaleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("The sale ID must be provided.");
    }
}