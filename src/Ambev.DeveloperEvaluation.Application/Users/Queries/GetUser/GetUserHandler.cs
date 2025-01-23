using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Serilog;

namespace Ambev.DeveloperEvaluation.Application.Users.Queries.GetUser;

/// <summary>
/// Handler for processing GetUserQuery requests
/// </summary>
public class GetUserHandler : IRequestHandler<GetUserQuery, GetUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetUserHandler
    /// </summary>
    /// <param name="userRepository">The user repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public GetUserHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetUserQuery request
    /// </summary>
    /// <param name="request">The GetUser query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user details if found</returns>
    public async Task<GetUserResult> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken)
    {
        var validator = new GetUserQueryValidator();

        var validationResult =
            await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(Environment.NewLine, validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

            Log.Error("Validation failed: {ValidationErrors}", message);

            throw new ValidationException(validationResult.Errors);
        }

        var user =
            await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            var message = $"User with ID {request.Id} not found";

            Log.Error(message);

            throw new KeyNotFoundException(message);
        }

        return _mapper.Map<GetUserResult>(user);
    }
}