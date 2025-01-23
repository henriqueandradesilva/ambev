using Ambev.DeveloperEvaluation.Common.Utils;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetListSale;

/// <summary>
/// Handles the request to retrieve a paginated list of sales.
/// </summary>
/// <remarks>
/// This class implements the CQRS pattern by acting as the request handler 
/// for the <see cref="GetListSaleQuery"/>. It retrieves the data from 
/// the <see cref="ISaleRepository"/> and maps it to the desired result format.
/// </remarks>
public class GetListSaleHandler : IRequestHandler<GetListSaleQuery, PaginatedList<GetListSaleResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetListSaleHandler"/> class.
    /// </summary>
    /// <param name="saleRepository">The repository for accessing sale data.</param>
    /// <param name="mapper">The mapper to convert entities to the result model.</param>
    public GetListSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the execution of the <see cref="GetListSaleQuery"/>.
    /// </summary>
    /// <param name="query">The query containing the pagination parameters.</param>
    /// <param name="cancellationToken">
    /// A token to cancel the operation if needed.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a 
    /// <see cref="PaginatedList{GetListSaleResult}"/> with the paginated sales.
    /// </returns>
    public async Task<PaginatedList<GetListSaleResult>> Handle(
        GetListSaleQuery query,
        CancellationToken cancellationToken)
    {
        var paginatedSales =
            await _saleRepository.GetListSaleWithPaginationAsync(query.PageNumber,
                                                                 query.PageSize,
                                                                 queryCustomizer: query => query.Include(s => s.ListSaleItems),
                                                                 cancellationToken: cancellationToken);

        var mappedSales = _mapper.Map<List<GetListSaleResult>>(paginatedSales);

        return new PaginatedList<GetListSaleResult>(mappedSales, paginatedSales.TotalCount, query.PageNumber, query.PageSize);
    }
}