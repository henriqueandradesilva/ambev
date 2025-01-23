using Ambev.DeveloperEvaluation.Application.Users.Queries.GetUser;
using Ambev.DeveloperEvaluation.Common.Utils;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.Queries.GetListUser;

/// <summary>
/// Handles the request to retrieve a paginated list of users.
/// </summary>
/// <remarks>
/// This class implements the CQRS pattern by acting as the request handler 
/// for the <see cref="GetListUserQuery"/>. It retrieves the data from 
/// the <see cref="IUserRepository"/> and maps it to the desired result format.
/// </remarks>
public class GetListUserHandler : IRequestHandler<GetListUserQuery, PaginatedList<GetListUserResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetListUserHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The repository for accessing user data.</param>
    /// <param name="mapper">The mapper to convert entities to the result model.</param>
    public GetListUserHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the execution of the <see cref="GetListUserQuery"/>.
    /// </summary>
    /// <param name="query">The query containing the pagination parameters.</param>
    /// <param name="cancellationToken">
    /// A token to cancel the operation if needed.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a 
    /// <see cref="PaginatedList{GetListUserResult}"/> with the paginated users.
    /// </returns>
    public async Task<PaginatedList<GetListUserResult>> Handle(
        GetListUserQuery query,
        CancellationToken cancellationToken)
    {
        var paginatedUsers = await _userRepository.GetListUserWithPaginationAsync(
            query.PageNumber,
            query.PageSize,
            queryCustomizer: query => query,
            cancellationToken: cancellationToken);

        var mappedUsers = _mapper.Map<List<GetListUserResult>>(paginatedUsers);

        return new PaginatedList<GetListUserResult>(mappedUsers, paginatedUsers.TotalCount, query.PageNumber, query.PageSize);
    }
}