using Ambev.DeveloperEvaluation.Common.Utils;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of the <see cref="ISaleRepository"/> interface using Entity Framework Core.
/// Provides methods to interact with the Sales data in the database.
/// </summary>
public class SaleRepository : Repository<Sale>, ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(
        DefaultContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PaginatedList<Sale>> GetListSaleWithPaginationAsync(
        int pageNumber,
        int pageSize,
        Func<IQueryable<Sale>, IQueryable<Sale>>? queryCustomizer = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Sale>()
                            .AsNoTracking();

        if (queryCustomizer != null)
            query = queryCustomizer(query);

        query = query.OrderBy(sale => sale.Customer);

        return await PaginatedList<Sale>.CreateAsync(query, pageNumber, pageSize, cancellationToken);
    }
}
