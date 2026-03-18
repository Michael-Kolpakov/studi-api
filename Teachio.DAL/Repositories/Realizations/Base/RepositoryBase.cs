using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Utils.Helpers;

namespace Teachio.DAL.Repositories.Realizations.Base;

/// <summary>
/// Represents the <see cref="RepositoryBase{T}"/> type.
/// </summary>
/// <typeparam name="T">The type of t.</typeparam>
public abstract class RepositoryBase<T> : IRepositoryBase<T>
    where T : class
{
    private readonly TeachioDbContext _dbContext;

    protected RepositoryBase(TeachioDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Finds the requested data.
    /// </summary>
    /// <param name="predicate">A predicate used to filter <paramref name="predicate"/>.</param>
    /// <param name="include">A value indicating whether <paramref name="include"/> is enabled.</param>
    /// <returns>The result produced by this operation.</returns>
    public IQueryable<T> FindAll(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        return GetQueryable(predicate, include).AsNoTracking();
    }

    /// <summary>
    /// Creates a new instance in the target store.
    /// </summary>
    /// <param name="entity">The <paramref name="entity"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public T Create(T entity)
    {
        return _dbContext.Set<T>().Add(entity).Entity;
    }

    /// <summary>
    /// Creates a new instance in the target store.
    /// </summary>
    /// <param name="entity">The <paramref name="entity"/> argument.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var entityEntry = await _dbContext.Set<T>().AddAsync(entity, cancellationToken);

        return entityEntry.Entity;
    }

    /// <summary>
    /// Creates a new instance in the target store.
    /// </summary>
    /// <param name="items">The <paramref name="items"/> argument.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task CreateRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<T>().AddRangeAsync(items, cancellationToken);
    }

    /// <summary>
    /// Updates the target entity.
    /// </summary>
    /// <param name="entity">The <paramref name="entity"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public T Update(T entity)
    {
        return _dbContext.Set<T>().Update(entity).Entity;
    }

    /// <summary>
    /// Updates the target entity.
    /// </summary>
    /// <param name="items">The <paramref name="items"/> argument.</param>
    public void UpdateRange(IEnumerable<T> items)
    {
        _dbContext.Set<T>().UpdateRange(items);
    }

    /// <summary>
    /// Deletes the target entity.
    /// </summary>
    /// <param name="entity">The <paramref name="entity"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public T Delete(T entity)
    {
        return _dbContext.Set<T>().Remove(entity).Entity;
    }

    /// <summary>
    /// Deletes the target entity.
    /// </summary>
    /// <param name="items">The <paramref name="items"/> argument.</param>
    public void DeleteRange(IEnumerable<T> items)
    {
        _dbContext.Set<T>().RemoveRange(items);
    }

    /// <summary>
    /// Performs the Attach operation.
    /// </summary>
    /// <param name="entity">The <paramref name="entity"/> argument.</param>
    public void Attach(T entity)
    {
        _dbContext.Set<T>().Attach(entity);
    }

    /// <summary>
    /// Performs the Entry operation.
    /// </summary>
    /// <param name="entity">The <paramref name="entity"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public EntityEntry<T> Entry(T entity)
    {
        return _dbContext.Entry(entity);
    }

    /// <summary>
    /// Performs the ExecuteSqlRaw operation.
    /// </summary>
    /// <param name="query">The query payload in <paramref name="query"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ExecuteSqlRaw(string query, CancellationToken cancellationToken = default)
    {
        return _dbContext.Database.ExecuteSqlRawAsync(query, cancellationToken);
    }

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="predicate">A predicate used to filter <paramref name="predicate"/>.</param>
    /// <param name="include">A value indicating whether <paramref name="include"/> is enabled.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(predicate, include).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="selector">A selector used to project <paramref name="selector"/>.</param>
    /// <param name="predicate">A predicate used to filter <paramref name="predicate"/>.</param>
    /// <param name="include">A value indicating whether <paramref name="include"/> is enabled.</param>
    /// <param name="ascendingSortKeySelector">A selector used to project <paramref name="ascendingSortKeySelector"/>.</param>
    /// <param name="descendingSortKeySelector">A selector used to project <paramref name="descendingSortKeySelector"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<PaginationResponse<T>> GetAllPaginatedAsync(
        ushort? pageNumber = null,
        ushort? pageSize = null,
        Expression<Func<T, T>>? selector = null,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Expression<Func<T, object>>? ascendingSortKeySelector = null,
        Expression<Func<T, object>>? descendingSortKeySelector = null,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable(
            predicate,
            include,
            selector,
            ascendingSortKeySelector,
            descendingSortKeySelector);

        var totalItems = await query.CountAsync(cancellationToken);

        IEnumerable<T> items;

        if (pageNumber is null && pageSize is null)
        {
            items = await query.ToListAsync(cancellationToken);
        }
        else if (pageNumber == 0 || pageSize == 0)
        {
            items = [];
        }
        else
        {
            var resolvedPageNumber = pageNumber ?? 1;
            var resolvedPageSize = pageSize ?? totalItems;
            var offset = (resolvedPageNumber - 1) * resolvedPageSize;

            items = await query
                .Skip(offset)
                .Take(resolvedPageSize)
                .ToListAsync(cancellationToken);
        }

        return PaginationResponse<T>.Create(items, totalItems, pageNumber, pageSize);
    }

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="predicate">A predicate used to filter <paramref name="predicate"/>.</param>
    /// <param name="include">A value indicating whether <paramref name="include"/> is enabled.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<T?> GetSingleOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(predicate, include).SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="predicate">A predicate used to filter <paramref name="predicate"/>.</param>
    /// <param name="include">A value indicating whether <paramref name="include"/> is enabled.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(predicate, include).FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="selector">A selector used to project <paramref name="selector"/>.</param>
    /// <param name="predicate">A predicate used to filter <paramref name="predicate"/>.</param>
    /// <param name="include">A value indicating whether <paramref name="include"/> is enabled.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, T>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(predicate, include, selector).FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Performs this operation.
    /// </summary>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <param name="selector">A selector used to project <paramref name="selector"/>.</param>
    /// <param name="predicate">A predicate used to filter <paramref name="predicate"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<TResult?> GetSingleOrDefaultProjectedAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.Select(selector).SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Performs this operation.
    /// </summary>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <param name="selector">A selector used to project <paramref name="selector"/>.</param>
    /// <param name="predicate">A predicate used to filter <paramref name="predicate"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<TResult?> GetFirstOrDefaultProjectedAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Performs this operation.
    /// </summary>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <param name="selector">A selector used to project <paramref name="selector"/>.</param>
    /// <param name="predicate">A predicate used to filter <paramref name="predicate"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<List<TResult>> GetProjectedListAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.Select(selector).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="selector">A selector used to project <paramref name="selector"/>.</param>
    /// <param name="predicate">A predicate used to filter <paramref name="predicate"/>.</param>
    /// <param name="include">A value indicating whether <paramref name="include"/> is enabled.</param>
    /// <param name="ascendingSortKeySelector">A selector used to project <paramref name="ascendingSortKeySelector"/>.</param>
    /// <param name="descendingSortKeySelector">A selector used to project <paramref name="descendingSortKeySelector"/>.</param>
    /// <param name="offset">The <paramref name="offset"/> argument.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, T>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Expression<Func<T, object>>? ascendingSortKeySelector = null,
        Expression<Func<T, object>>? descendingSortKeySelector = null,
        int? offset = null,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(
                predicate,
                include,
                selector,
                ascendingSortKeySelector,
                descendingSortKeySelector,
                offset: offset)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the requested data.
    /// </summary>
    /// <param name="predicate">A predicate used to filter <paramref name="predicate"/>.</param>
    /// <param name="include">A value indicating whether <paramref name="include"/> is enabled.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<int> GetSelfCountAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable(predicate, include);
        var count = await query.CountAsync(cancellationToken);

        return count;
    }

    /// <summary>
    /// Performs this operation.
    /// </summary>
    /// <typeparam name="TProperty">The type of property.</typeparam>
    /// <param name="collectionSelector">A selector used to project <paramref name="collectionSelector"/>.</param>
    /// <param name="predicate">A predicate used to filter <paramref name="predicate"/>.</param>
    /// <param name="include">A value indicating whether <paramref name="include"/> is enabled.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<int> GetNavigationCollectionsCountAsync<TProperty>(
        Expression<Func<T, IEnumerable<TProperty>>> collectionSelector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable(predicate, include);
        var elementsCount = await query.SelectMany(collectionSelector).CountAsync(cancellationToken);

        return elementsCount;
    }

    public async Task<Dictionary<TKey, int>> GetNavigationCollectionsCountsAsync<TKey, TProperty>(
        Expression<Func<T, TKey>> keySelector,
        Expression<Func<T, IEnumerable<TProperty>>> collectionSelector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = default)
        where TKey : notnull
    {
        var query = GetQueryable(predicate, include);
        var projection = BuildNavigationProjection(query, keySelector, collectionSelector);

        return await projection
            .SelectMany(
                x => x.Collection.DefaultIfEmpty(),
                (projectionItem, collectionItem) => new { projectionItem.Key, CollectionItem = collectionItem })
            .GroupBy(x => x.Key)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Count(item => item.CollectionItem != null),
                cancellationToken);
    }

    private static IQueryable<NavigationCollectionProjection<TKey, TProperty>> BuildNavigationProjection<TKey, TProperty>(
        IQueryable<T> query,
        Expression<Func<T, TKey>> keySelector,
        Expression<Func<T, IEnumerable<TProperty>>> collectionSelector)
    {
        var entityParameter = Expression.Parameter(typeof(T), "entity");
        var rewrittenKeySelector = ReplaceParameter(keySelector, entityParameter);
        var rewrittenCollectionSelector = ReplaceParameter(collectionSelector, entityParameter);

        var projectionConstructor = typeof(NavigationCollectionProjection<TKey, TProperty>)
            .GetConstructor([typeof(TKey), typeof(IEnumerable<TProperty>)])!;

        var projectionBody = Expression.New(
            projectionConstructor,
            rewrittenKeySelector.Body,
            rewrittenCollectionSelector.Body);

        var projectionLambda = Expression.Lambda<Func<T, NavigationCollectionProjection<TKey, TProperty>>>(
            projectionBody,
            entityParameter);

        return query.Select(projectionLambda);
    }

    private static Expression<Func<T, TResult>> ReplaceParameter<TResult>(
        Expression<Func<T, TResult>> expression,
        ParameterExpression newParameter)
    {
        var visitor = new ReplaceParameterVisitor(expression.Parameters[0], newParameter);
        var updatedBody = visitor.Visit(expression.Body);

        return Expression.Lambda<Func<T, TResult>>(updatedBody, newParameter);
    }

    private sealed record NavigationCollectionProjection<TKey, TProperty>(
        TKey Key,
        IEnumerable<TProperty> Collection);

    private sealed class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _source;
        private readonly ParameterExpression _target;

        public ReplaceParameterVisitor(ParameterExpression source, ParameterExpression target)
        {
            _source = source;
            _target = target;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _source ? _target : base.VisitParameter(node);
        }
    }

    private IQueryable<T> GetQueryable(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Expression<Func<T, T>>? selector = null,
        Expression<Func<T, object>>? ascendingSortKeySelector = null,
        Expression<Func<T, object>>? descendingSortKeySelector = null,
        int? limit = null,
        int? offset = null)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (include is not null)
        {
            query = include(query);
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (selector is not null)
        {
            query = query.Select(selector);
        }

        if (ascendingSortKeySelector is not null)
        {
            query = query.OrderBy(ascendingSortKeySelector);
        }

        if (descendingSortKeySelector is not null)
        {
            query = query.OrderByDescending(descendingSortKeySelector);
        }

        if (offset is >= 0)
        {
            query = query.Skip(offset.Value);
        }

        if (limit is > 0)
        {
            query = query.Take(limit.Value);
        }

        return query.AsNoTracking();
    }
}
