using Ambev.DeveloperEvaluation.Application.Users.Queries.GetUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;

/// <summary>
/// Profile for mapping GetUser feature requests to queries
/// </summary>
public class GetUserProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetUser feature
    /// </summary>
    public GetUserProfile()
    {
        CreateMap<User, GetUserResult>();

        CreateMap<GetUserRequest, GetUserQuery>();
        CreateMap<GetUserResult, GetUserResponse>();

        CreateMap<Guid, GetUserQuery>()
            .ConstructUsing(id => new GetUserQuery(id));
    }
}