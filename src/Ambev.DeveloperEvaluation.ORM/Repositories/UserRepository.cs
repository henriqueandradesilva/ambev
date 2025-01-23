using Ambev.DeveloperEvaluation.Common.Utils;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of IUserRepository using Entity Framework Core
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    private readonly DefaultContext _context;

    public UserRepository(
        DefaultContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a paginated list of users based on the specified criteria.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <param name="queryCustomizer">
    /// An optional function to customize the query before execution. 
    /// This can be used for filtering, sorting, or additional conditions.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the operation. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a <see cref="PaginatedList{User}"/>
    /// with the retrieved users.
    /// </returns>
    public async Task<PaginatedList<User>> GetListUserWithPaginationAsync(
        int pageNumber,
        int pageSize,
        Func<IQueryable<User>, IQueryable<User>>? queryCustomizer = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<User>()
                            .AsNoTracking();

        if (queryCustomizer != null)
            query = queryCustomizer(query);

        return await PaginatedList<User>.CreateAsync(query, pageNumber, pageSize, cancellationToken);
    }
}