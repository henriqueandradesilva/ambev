using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSale;

/// <summary>
/// Validator for GetSaleQueryValidator
/// </summary>
public class GetSaleQueryValidator : AbstractValidator<GetSaleQuery>
{
    /// <summary>
    /// Initializes validation rules for GetSaleQueryValidator
    /// </summary>
    public GetSaleQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required");
    }
}