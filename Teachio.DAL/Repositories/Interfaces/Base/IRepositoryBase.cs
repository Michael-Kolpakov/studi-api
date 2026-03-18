using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.DAL.Repositories.Interfaces.Base;

/// <summary>
/// Defines the contract for <see cref="IRepositoryBase{T}"/>.
/// </summary>
/// <typeparam name="T">The entity type handled by this repository.</typeparam>
public interface IRepositoryBase<T>
    where T : class
{
    /// <summary>
    /// Adds a new entity to the current context.
    /// </summary>
    /// <param name="entity">The entity instance to add.</param>
    /// <returns>The tracked entity instance.</returns>
    T Create(T entity);

    /// <summary>
    /// Adds a new entity to the current context asynchronously.
    /// </summary>
    /// <param name="entity">The entity instance to add.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the tracked entity instance.</returns>
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities to the current context asynchronously.
    /// </summary>
    /// <param name="items">The entity instances to add.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CreateRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an entity as modified in the current context.
    /// </summary>
    /// <param name="entity">The entity instance to update.</param>
    /// <returns>The tracked entity instance.</returns>
    T Update(T entity);

    /// <summary>
    /// Marks multiple entities as modified in the current context.
    /// </summary>
    /// <param name="items">The entity instances to update.</param>
    void UpdateRange(IEnumerable<T> items);

    /// <summary>
    /// Marks an entity for deletion in the current context.
    /// </summary>
    /// <param name="entity">The entity instance to delete.</param>
    /// <returns>The tracked entity instance.</returns>
    T Delete(T entity);

    /// <summary>
    /// Marks multiple entities for deletion in the current context.
    /// </summary>
    /// <param name="items">The entity instances to delete.</param>
    void DeleteRange(IEnumerable<T> items);

    /// <summary>
    /// Attaches a detached entity to the current context.
    /// </summary>
    /// <param name="entity">The entity instance to attach.</param>
    void Attach(T entity);

    /// <summary>
    /// Returns the change-tracking entry for the specified entity.
    /// </summary>
    /// <param name="entity">The entity instance to inspect.</param>
    /// <returns>The entity entry associated with <paramref name="entity"/>.</returns>
    EntityEntry<T> Entry(T entity);

    /// <summary>
    /// Executes a raw SQL command.
    /// </summary>
    /// <param name="query">The raw SQL command to execute.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ExecuteSqlRaw(string query, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all entities that satisfy the optional filter and include expressions.
    /// </summary>
    /// <param name="predicate">An optional filter expression.</param>
    /// <param name="include">An optional include expression for related data.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the resulting entities.</returns>
    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a paginated set of entities with optional filtering, projection, includes, and sorting.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="selector">An optional projection expression.</param>
    /// <param name="predicate">An optional filter expression.</param>
    /// <param name="include">An optional include expression for related data.</param>
    /// <param name="ascendingSortKeySelector">An optional ascending sort key selector.</param>
    /// <param name="descendingSortKeySelector">An optional descending sort key selector.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the paginated response.</returns>
    Task<PaginationResponse<T>> GetAllPaginatedAsync(
        ushort? pageNumber = null,
        ushort? pageSize = null,
        Expression<Func<T, T>>? selector = null,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Expression<Func<T, object>>? ascendingSortKeySelector = null,
        Expression<Func<T, object>>? descendingSortKeySelector = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single entity that matches the specified criteria, or <see langword="null"/>.
    /// </summary>
    /// <param name="predicate">An optional filter expression.</param>
    /// <param name="include">An optional include expression for related data.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the matching entity, if found.</returns>
    Task<T?> GetSingleOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first entity that matches the specified criteria, or <see langword="null"/>.
    /// </summary>
    /// <param name="predicate">An optional filter expression.</param>
    /// <param name="include">An optional include expression for related data.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the first matching entity, if found.</returns>
    Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first projected entity that matches the specified criteria, or <see langword="null"/>.
    /// </summary>
    /// <param name="selector">The projection expression to apply.</param>
    /// <param name="predicate">An optional filter expression.</param>
    /// <param name="include">An optional include expression for related data.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the first matching projected entity, if found.</returns>
    Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, T>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first projected entity with optional filtering, sorting, and offset, or <see langword="null"/>.
    /// </summary>
    /// <param name="selector">The projection expression to apply.</param>
    /// <param name="predicate">An optional filter expression.</param>
    /// <param name="include">An optional include expression for related data.</param>
    /// <param name="ascendingSortKeySelector">An optional ascending sort key selector.</param>
    /// <param name="descendingSortKeySelector">An optional descending sort key selector.</param>
    /// <param name="offset">An optional number of items to skip.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the first matching projected entity, if found.</returns>
    Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, T>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Expression<Func<T, object>>? ascendingSortKeySelector = null,
        Expression<Func<T, object>>? descendingSortKeySelector = null,
        int? offset = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single projected value that matches the specified criteria, or <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TResult">The projection result type.</typeparam>
    /// <param name="selector">The projection expression to apply.</param>
    /// <param name="predicate">An optional filter expression.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the projected value, if found.</returns>
    Task<TResult?> GetSingleOrDefaultProjectedAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first projected value that matches the specified criteria, or <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TResult">The projection result type.</typeparam>
    /// <param name="selector">The projection expression to apply.</param>
    /// <param name="predicate">An optional filter expression.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the first projected value, if found.</returns>
    Task<TResult?> GetFirstOrDefaultProjectedAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a projected list of values for entities that match the specified criteria.
    /// </summary>
    /// <typeparam name="TResult">The projection result type.</typeparam>
    /// <param name="selector">The projection expression to apply.</param>
    /// <param name="predicate">An optional filter expression.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the projected list.</returns>
    Task<List<TResult>> GetProjectedListAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the number of entities that match the specified criteria.
    /// </summary>
    /// <param name="predicate">An optional filter expression.</param>
    /// <param name="include">An optional include expression for related data.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the number of matching entities.</returns>
    Task<int> GetSelfCountAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the total count of items from a navigation collection across matching entities.
    /// </summary>
    /// <typeparam name="TProperty">The type of items in the navigation collection.</typeparam>
    /// <param name="collectionSelector">The navigation collection selector.</param>
    /// <param name="predicate">An optional filter expression.</param>
    /// <param name="include">An optional include expression for related data.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the total collection item count.</returns>
    Task<int> GetNavigationCollectionsCountAsync<TProperty>(
        Expression<Func<T, IEnumerable<TProperty>>> collectionSelector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns navigation collection counts grouped by a key for matching entities.
    /// </summary>
    /// <typeparam name="TKey">The grouping key type.</typeparam>
    /// <typeparam name="TProperty">The type of items in the navigation collection.</typeparam>
    /// <param name="keySelector">The key selector used for grouping.</param>
    /// <param name="collectionSelector">The navigation collection selector.</param>
    /// <param name="predicate">An optional filter expression.</param>
    /// <param name="include">An optional include expression for related data.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains a dictionary of counts by key.</returns>
    Task<Dictionary<TKey, int>> GetNavigationCollectionsCountsAsync<TKey, TProperty>(
        Expression<Func<T, TKey>> keySelector,
        Expression<Func<T, IEnumerable<TProperty>>> collectionSelector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
        where TKey : notnull;

    /// <summary>
    /// Returns a queryable sequence for entities that match the specified criteria.
    /// </summary>
    /// <param name="predicate">An optional filter expression.</param>
    /// <param name="include">An optional include expression for related data.</param>
    /// <returns>A queryable sequence for further composition.</returns>
    IQueryable<T> FindAll(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
}
