using Ambev.DeveloperEvaluation.Application.Users.Queries.GetUser;
using Ambev.DeveloperEvaluation.Common.Utils;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.Queries.GetListUser;

/// <summary>
/// Query to retrieve a paginated list of users.
/// </summary>
/// <remarks>
/// This query is part of the CQRS pattern and is handled by a mediator.
/// It encapsulates the pagination parameters to fetch a specific page of users.
/// </remarks>
public class GetListUserQuery : IRequest<PaginatedList<GetListUserResult>>
{
    /// <summary>
    /// Gets or sets the current page number for the paginated result.
    /// Defaults to 1.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the size of the page for the paginated result.
    /// Defaults to 10.
    /// </summary>
    public int PageSize { get; set; } = 10;
}