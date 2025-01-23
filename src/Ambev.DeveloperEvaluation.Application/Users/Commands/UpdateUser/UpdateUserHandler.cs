using Ambev.DeveloperEvaluation.Common.Security.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Serilog;

namespace Ambev.DeveloperEvaluation.Application.Users.Commands.UpdateUser;

/// <summary>
/// Handler for processing UpdateUserCommand requests
/// </summary>
public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Initializes a new instance of UpdateUserHandler
    /// </summary>
    /// <param name="userRepository">The user repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public UpdateUserHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Handles the UpdateUserCommand request
    /// </summary>
    /// <param name="command">The UpdateUser command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The Updated user details</returns>
    public async Task<UpdateUserResult> Handle(
        UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new UpdateUserCommandValidator();

        var validationResult =
            await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(Environment.NewLine, validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

            Log.Error("Validation failed: {ValidationErrors}", message);

            throw new ValidationException(validationResult.Errors);
        }

        var existingUser =
            await _userRepository.GetByIdAsync(command.Id, cancellationToken);

        if (existingUser == null)
        {
            var message = $"User with Id {command.Id} not found";

            Log.Error(message);

            throw new InvalidOperationException(message);
        }

        _mapper.Map(command, existingUser);

        existingUser.Password = _passwordHasher.HashPassword(command.Password);

        existingUser.MarkAsUpdated();

        var updatedUser = await _userRepository.UpdateAsync(existingUser, cancellationToken);

        var result = _mapper.Map<UpdateUserResult>(updatedUser);

        return result;
    }
}