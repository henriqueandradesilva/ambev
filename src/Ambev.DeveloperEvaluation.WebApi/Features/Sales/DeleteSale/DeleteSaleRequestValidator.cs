using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

/// <summary>
/// Validator for the DeleteSaleRequest to ensure data integrity.
/// </summary>
public class DeleteSaleRequestValidator : AbstractValidator<DeleteSaleRequest>
{
    /// <summary>
    /// Configures validation rules for the DeleteSaleRequest.
    /// </summary>
    public DeleteSaleRequestValidator()
    {
        // Ensures the Id property is not empty.
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("The sale identifier must be provided.");
    }
}
