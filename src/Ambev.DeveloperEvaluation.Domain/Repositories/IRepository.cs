using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Generic repository interface for performing CRUD operations on entities.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The entity if found, or null if not.</returns>
    Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<T?> GetByIdWithIncludeAsync(
        Guid id,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Finds a single entity based on a specified condition.
    /// </summary>
    /// <param name="predicate">An expression defining the condition to filter the entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A single entity matching the condition, or null if no entity is found.</returns>
    Task<T> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities based on a specified condition.
    /// </summary>
    /// <param name="predicate">An expression defining the condition to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of entities matching the condition.</returns>
    Task<IEnumerable<T>> FindListAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all entities from the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of all entities.</returns>
    Task<IEnumerable<T>> GetAllAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new entity in the database.
    /// </summary>
    /// <param name="entity">The entity to be created.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created entity.</returns>
    Task<T> CreateAsync(
        T entity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity with updated values.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated entity.</returns>
    Task<T> UpdateAsync(
        T entity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the entity was deleted, false otherwise.</returns>
    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}