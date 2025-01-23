using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of the <see cref="ISaleItemRepository"/> interface using Entity Framework Core.
/// Provides methods to manage sale items in the database.
/// </summary>
public class SaleItemRepository : Repository<SaleItem>, ISaleItemRepository
{
    private readonly DefaultContext _context;

    public SaleItemRepository(
        DefaultContext context) : base(context)
    {
        _context = context;
    }
}