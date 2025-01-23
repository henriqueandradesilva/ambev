using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Specifications.Interfaces;

namespace Ambev.DeveloperEvaluation.Domain.Specifications;

public class ActiveUserSpecification : ISpecification<User>
{
    public bool IsSatisfiedBy(
        User user)
    {
        return user.Status == UserStatus.Active;
    }
}