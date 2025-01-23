using Ambev.DeveloperEvaluation.Common.Utils;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Defines the contract for accessing and managing <see cref="Sale"/> entities in the repository.
/// </summary>
public interface ISaleRepository : IRepository<Sale>
{
    Task<PaginatedList<Sale>> GetListSaleWithPaginationAsync(
        int pageNumber,
        int pageSize,
        Func<IQueryable<Sale>, IQueryable<Sale>>? queryCustomizer = null,
        CancellationToken cancellationToken = default);
}