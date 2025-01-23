using Ambev.DeveloperEvaluation.Common.Utils;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetListSale;

/// <summary>
/// Query to retrieve a paginated list of sales.
/// </summary>
/// <remarks>
/// This query is part of the CQRS pattern and is handled by a mediator.
/// It encapsulates the pagination parameters to fetch a specific page of sales.
/// </remarks>
public class GetListSaleQuery : IRequest<PaginatedList<GetListSaleResult>>
{
    public GetListSaleQuery(
        int pageNumber,
        int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

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