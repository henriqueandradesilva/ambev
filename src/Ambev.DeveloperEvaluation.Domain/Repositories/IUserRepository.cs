using Ambev.DeveloperEvaluation.Common.Utils;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for managing operations related to <see cref="User"/> entities.
/// </summary>
/// <remarks>
/// Extends the generic <see cref="IRepository{T}"/> to include additional functionality 
/// for retrieving paginated user lists.
/// </remarks>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Retrieves a paginated list of users based on the specified criteria.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <param name="queryCustomizer">
    /// An optional function to customize the query before execution.
    /// This can be used for filtering, sorting, or applying additional conditions.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the operation. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a <see cref="PaginatedList{User}"/>
    /// with the retrieved users.
    /// </returns>
    Task<PaginatedList<User>> GetListUserWithPaginationAsync(
        int pageNumber,
        int pageSize,
        Func<IQueryable<User>, IQueryable<User>>? queryCustomizer = null,
        CancellationToken cancellationToken = default);
}
