using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Users.Queries.GetUser;

/// <summary>
/// Validator for GetUserQueryValidator
/// </summary>
public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
    /// <summary>
    /// Initializes validation rules for GetUserQueryValidator
    /// </summary>
    public GetUserQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}