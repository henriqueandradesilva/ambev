using Ambev.DeveloperEvaluation.Common.Security.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using MediatR;
using Serilog;

namespace Ambev.DeveloperEvaluation.Application.Auth.Commands.AuthenticateUser;

public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserCommand, AuthenticateUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthenticateUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthenticateUserResult> Handle(
        AuthenticateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _userRepository.FindAsync(c => c.Email == request.Email, cancellationToken);

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.Password))
        {
            var message = $"Invalid credentials {request.Email}";

            Log.Error(message);

            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var activeUserSpec = new ActiveUserSpecification();

        if (!activeUserSpec.IsSatisfiedBy(user))
        {
            var message = $"User is not active: {user.Username}";

            Log.Error(message);

            throw new UnauthorizedAccessException(message);
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthenticateUserResult
        {
            Token = token,
            Email = user.Email,
            Name = user.Username,
            Role = user.Role.ToString()
        };
    }
}