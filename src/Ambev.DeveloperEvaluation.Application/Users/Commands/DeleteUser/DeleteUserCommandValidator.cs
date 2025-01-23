using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Users.Commands.DeleteUser;

/// <summary>
/// Validator for DeleteUserCommandValidator
/// </summary>
public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    /// <summary>
    /// Initializes validation rules for DeleteUserCommandValidator
    /// </summary>
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}