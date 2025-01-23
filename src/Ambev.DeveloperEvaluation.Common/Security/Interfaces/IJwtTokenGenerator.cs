namespace Ambev.DeveloperEvaluation.Common.Security.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(
        IUser user);
}