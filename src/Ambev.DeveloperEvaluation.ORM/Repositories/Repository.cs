using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Generic repository implementation for performing CRUD operations using Entity Framework.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{T}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public Repository(
        DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The entity if found, or null if not.</returns>
    public async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<T?> GetByIdWithIncludeAsync(
        Guid id,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
            query = query.Include(include);

        return await query.FirstOrDefaultAsync(entity => EF.Property<Guid>(entity, "Id") == id, cancellationToken);
    }

    public async Task<T> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Finds entities based on a specified condition.
    /// </summary>
    /// <param name="predicate">An expression defining the condition to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of entities matching the condition.</returns>
    public async Task<IEnumerable<T>> FindListAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all entities from the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of all entities.</returns>
    public async Task<IEnumerable<T>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Creates a new entity in the database.
    /// </summary>
    /// <param name="entity">The entity to be created.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created entity.</returns>
    public async Task<T> CreateAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbSet.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return entity;
        }
        catch (DbUpdateException ex)
        {
            throw DbUpdateExceptionHandler(ex);
        }
    }

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity with updated values.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated entity.</returns>
    public async Task<T> UpdateAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _dbSet.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return entity;
        }
        catch (DbUpdateException ex)
        {
            throw DbUpdateExceptionHandler(ex);
        }
    }

    /// <summary>
    /// Deletes an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the entity was deleted, false otherwise.</returns>
    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);

        if (entity == null)
            return false;

        try
        {
            _dbSet.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (DbUpdateException ex)
        {
            throw DbUpdateExceptionHandler(ex);
        }
    }

    private InvalidOperationException DbUpdateExceptionHandler(
        DbUpdateException ex)
    {
        var message = string.Empty;

        var sqlException = ex.InnerException as NpgsqlException;

        if (sqlException != null &&
            sqlException.SqlState == "23503")
        {
            var deletedEntries =
                _context.ChangeTracker?.Entries()
                                      ?.Where(e => e.State == EntityState.Deleted)
                                      ?.ToList();

            if (deletedEntries.Any())
            {
                message = "Erro ao tentar excluir: existem registros dependentes que precisam ser removidos primeiro.";

                Log.Error(message);

                throw new InvalidOperationException(message);
            }

            message = "Erro ao tentar inserir/atualizar: chave estrangeira não encontrada.";

            Log.Error(message);

            throw new InvalidOperationException(message);
        }
        else if (sqlException.SqlState == "23505")
        {
            message = "Erro ao tentar inserir/atualizar: violação de índice exclusivo.";

            Log.Error(message);

            throw new InvalidOperationException(message);
        }

        var detailedMessage = ex.InnerException?.Message ?? ex.Message;

        message = $"Erro inesperado: {detailedMessage}\n{ex.StackTrace}";

        Log.Error(message);

        throw new InvalidOperationException(message);
    }
}