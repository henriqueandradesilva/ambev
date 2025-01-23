using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Serilog;

namespace Ambev.DeveloperEvaluation.Application.Users.Commands.DeleteUser;

/// <summary>
/// Handler for processing DeleteUserCommand requests
/// </summary>
public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, DeleteUserResult>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of DeleteUserHandler
    /// </summary>
    /// <param name="userRepository">The user repository</param>
    public DeleteUserHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Handles the DeleteUserCommand request
    /// </summary>
    /// <param name="request">The DeleteUser command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteUserResult> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var validator = new DeleteUserCommandValidator();

        var validationResult =
            await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(Environment.NewLine, validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

            Log.Error("Validation failed: {ValidationErrors}", message);

            throw new ValidationException(validationResult.Errors);
        }

        var success =
            await _userRepository.DeleteAsync(request.Id, cancellationToken);

        if (!success)
        {
            var message = $"Sale with ID {request.Id} not found";

            Log.Error(message);

            throw new KeyNotFoundException(message);
        }

        return new DeleteUserResult()
        {
            Success = success,
            Message = "The user was successfully deleted."
        };
    }
}