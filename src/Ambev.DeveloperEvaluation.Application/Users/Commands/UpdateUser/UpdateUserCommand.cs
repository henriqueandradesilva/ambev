using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.Commands.UpdateUser;

/// <summary>
/// Command for updating an existing user.
/// </summary>
/// <remarks>
/// This command is used to capture the required data for updating a user, 
/// including id, username, password, phone number, email, status, and role. 
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request 
/// that returns a <see cref="UpdateUserResult"/>.
/// 
/// The data provided in this command is validated using the 
/// <see cref="UpdateUserCommandValidator"/> which extends 
/// <see cref="AbstractValidator{T}"/> to ensure that the fields are correctly 
/// populated and follow the required rules.
/// </remarks>
public class UpdateUserCommand : IRequest<UpdateUserResult>
{
    /// <summary>
    /// Gets or sets the unique identifier of the user to be updated.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the email address of the Company to be updated.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the username of the user to be updated.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password of the Company to be updated.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number of the Company to be updated.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of the Company to be updated.
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the role of the Company to be updated.
    /// </summary>
    public UserRole Role { get; set; }

    public ValidationResultDetail Validate()
    {
        var validator = new UpdateUserCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}