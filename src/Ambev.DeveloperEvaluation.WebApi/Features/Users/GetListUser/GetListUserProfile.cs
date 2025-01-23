using Ambev.DeveloperEvaluation.Application.Users.Queries.GetListUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.GetListUser;

/// <summary>
/// Profile for AutoMapper configuration for Users related mappings.
/// </summary>
public class GetListUserProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetListUserProfile"/> class.
    /// </summary>
    public GetListUserProfile()
    {
        CreateMap<User, GetListUserResult>();
        CreateMap<GetListUserResult, GetListUserResponse>();
    }
}